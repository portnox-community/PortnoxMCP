using ModelContextProtocol.Server;
using System.ComponentModel;
using ModelContextProtocol.AspNetCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;

namespace PortnoxMCP.Tools
{
    [McpServerToolType]
    public sealed class GetPortnoxMACAccounts
    {
        private readonly PortnoxApiClient _client;
        private readonly ILogger<GetPortnoxMACAccounts> _logger;

        public GetPortnoxMACAccounts(PortnoxApiClient client, ILogger<GetPortnoxMACAccounts> logger)
        {
            _client = client;
            _logger = logger;
        }

        /// <summary>
        /// Retrieves all MAC-based accounts from the Portnox API. Supports filtering by name.
        /// </summary>
        [McpServerTool(
            Title = "get_portnox_mac_accounts",
            ReadOnly = true,
            Idempotent = true,
            Destructive = false
        )]
        [Description("Retrieves all MAC-based accounts from the Portnox API. Supports filtering by account name. Handles pagination automatically.")]
    public async Task<List<object>> GetMACAccountsAsync(string? name = null)
        {
            _logger.LogInformation("VERBOSE: Starting MAC Accounts retrieval. Starting at page index: 1");
            var macAccounts = new List<JsonElement>();
            int pageIdx = 1;
            int consecutiveFailures = 0;
            const int maxConsecutiveFailures = 3;
            const int maxRetries = 3;
            const int retryDelayMs = 2000;
            while (true)
            {
                _logger.LogInformation($"VERBOSE: Requesting page {pageIdx}");
                string requestUri = $"api/list-mac-based-accounts/{pageIdx}";
                HttpResponseMessage? resp = null;
                int retries = 0;
                while (retries < maxRetries)
                {
                    // Create a new HttpRequestMessage for each attempt
                    var req = new HttpRequestMessage(HttpMethod.Get, requestUri);
                    _logger.LogInformation($"VERBOSE: Method: {req.Method} URI: {{_client.BaseAddress}}{req.RequestUri} Body: null Headers: {{ {string.Join(", ", req.Headers)} }}");
                    try
                    {
                        resp = await _client.SendAsync(req);
                        if ((int)resp.StatusCode == 503)
                        {
                            _logger.LogWarning($"WARNING: Received 503 ServiceUnavailable. Retrying in {retryDelayMs}ms...");
                            await Task.Delay(retryDelayMs);
                            retries++;
                            continue;
                        }
                        break;
                    }
                    catch (HttpRequestException ex)
                    {
                        _logger.LogWarning($"WARNING: HTTP request failed: {ex.Message}. Retrying in {retryDelayMs}ms...");
                        await Task.Delay(retryDelayMs);
                        retries++;
                    }
                }
                if (resp == null)
                {
                    _logger.LogWarning($"WARNING: Failed to get a response after {maxRetries} retries. Stopping.");
                    break;
                }

                var contentType = resp.Content.Headers.ContentType?.ToString() ?? "unknown";
                var encoding = resp.Content.Headers.ContentEncoding.ToString();
                var payload = await resp.Content.ReadAsByteArrayAsync();
                var json = System.Text.Encoding.UTF8.GetString(payload);
                _logger.LogInformation($"VERBOSE: GET {{_client.BaseAddress}}{requestUri} with {payload.Length}-byte payload");
                _logger.LogInformation($"VERBOSE: received {payload.Length}-byte response of content type {contentType}");
                _logger.LogInformation($"VERBOSE: Content encoding: {encoding}");

                JsonDocument doc;
                try
                {
                    doc = JsonDocument.Parse(json);
                    consecutiveFailures = 0; // reset on success
                }
                catch (JsonException ex)
                {
                    string truncated = json.Length > 500 ? json.Substring(0, 500) + "... [truncated]" : json;
                    _logger.LogWarning($"WARNING: Failed to parse JSON response: {ex.Message}\nResponse body (truncated): {truncated}");
                    consecutiveFailures++;
                    if (consecutiveFailures >= maxConsecutiveFailures)
                    {
                        _logger.LogWarning($"WARNING: Reached {maxConsecutiveFailures} consecutive parse failures. Stopping.");
                        break;
                    }
                    else
                    {
                        pageIdx++;
                        continue;
                    }
                }

                if (!doc.RootElement.TryGetProperty("MabAccounts", out var accounts) || accounts.ValueKind == JsonValueKind.Null)
                {
                    _logger.LogInformation($"VERBOSE: No more results at page {pageIdx}");
                    break;
                }

                int count = 0;
                foreach (var acc in accounts.EnumerateArray())
                {
                    macAccounts.Add(acc);
                    count++;
                }
                _logger.LogInformation($"VERBOSE: Found {count} results on page {pageIdx}");
                if (count == 0)
                {
                    break;
                }
                pageIdx++;
            }

            List<JsonElement> filteredAccounts = macAccounts;
            if (!string.IsNullOrEmpty(name))
            {
                _logger.LogInformation($"VERBOSE: Filtering MAC accounts by name: {name}");
                filteredAccounts = macAccounts.FindAll(a =>
                {
                    if (a.TryGetProperty("AccountName", out var accountName))
                        return accountName.GetString()?.Contains(name, System.StringComparison.OrdinalIgnoreCase) == true;
                    return false;
                });
            }
            _logger.LogInformation($"VERBOSE: Returning {filteredAccounts.Count} MAC accounts");
            // Return as List<object> for compatibility
            return filteredAccounts.ConvertAll(a => (object)a);
        }
    }
}
