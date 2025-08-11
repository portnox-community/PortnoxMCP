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
        public async Task<List<object>> GetMACAccountsAsync(string name = null)
        {
            _logger.LogDebug("[GetMACAccountsAsync] Invoked with name={Name}", name);
            var macAccounts = new List<object>();
            int pageIdx = 1;
            while (true)
            {
                _logger.LogDebug("[GetMACAccountsAsync] Fetching MAC accounts page {PageIdx}", pageIdx);
                var req = new HttpRequestMessage(HttpMethod.Get, $"/api/list-mac-based-accounts/{pageIdx}");
                var resp = await _client.SendAsync(req);
                var json = await resp.Content.ReadAsStringAsync();
                var doc = JsonDocument.Parse(json);
                if (!doc.RootElement.TryGetProperty("mabAccounts", out var accounts) || accounts.ValueKind == JsonValueKind.Null)
                {
                    _logger.LogDebug("[GetMACAccountsAsync] No more results at page {PageIdx}", pageIdx);
                    break;
                }
                foreach (var acc in accounts.EnumerateArray())
                    macAccounts.Add(acc);
                pageIdx++;
            }
            if (!string.IsNullOrEmpty(name))
            {
                _logger.LogDebug("[GetMACAccountsAsync] Filtering MAC accounts by name: {Name}", name);
                macAccounts = macAccounts.FindAll(a => a.ToString().Contains(name));
            }
            _logger.LogDebug("[GetMACAccountsAsync] Returning {Count} MAC accounts", macAccounts.Count);
            return macAccounts;
        }
    }
}
