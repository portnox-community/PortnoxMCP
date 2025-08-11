using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace PortnoxMCP
{
    public class PortnoxApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<PortnoxApiClient> _logger;
        private readonly string _apiKey;
    private readonly int _maxRetries;
    private readonly TimeSpan _initialDelay;

        public PortnoxApiClient(HttpClient httpClient, ILogger<PortnoxApiClient> logger, IConfiguration config)
        {
            _httpClient = httpClient;
            _logger = logger;
            _apiKey = (config["PORTNOXAPIKEY"] ?? Environment.GetEnvironmentVariable("PORTNOXAPIKEY") ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(_apiKey))
                throw new InvalidOperationException("PORTNOXAPIKEY environment variable is not set.");
            // Ensure BaseAddress is set
            if (_httpClient.BaseAddress == null)
                _httpClient.BaseAddress = new Uri("https://clear.portnox.com:8081/CloudPortalBackEnd/");

            // Read max retries from env/config, default to 3
            var maxRetriesStr = config["PORTNOX_MAX_RETRIES"] ?? Environment.GetEnvironmentVariable("PORTNOX_MAX_RETRIES");
            if (!int.TryParse(maxRetriesStr, out _maxRetries) || _maxRetries < 0)
                _maxRetries = 3;

            // Read initial delay (seconds) from env/config, default to 1 second
            var delayStr = config["PORTNOX_INITIAL_DELAY_SECONDS"] ?? Environment.GetEnvironmentVariable("PORTNOX_INITIAL_DELAY_SECONDS");
            if (!double.TryParse(delayStr, out double delaySeconds) || delaySeconds <= 0)
                delaySeconds = 1;
            _initialDelay = TimeSpan.FromSeconds(delaySeconds);

            // Enforce TLS 1.2+
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls13;
        }

        public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, bool verbose = false)
        {
            // Set the Authorization header to 'Bearer <API_KEY>'
            request.Headers.Remove("Authorization");
            var trimmedKey = _apiKey.Trim();
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", trimmedKey);
            _logger.LogInformation("[SendAsync] Authorization header set to Bearer (redacted): {ApiKey}", trimmedKey.Length > 6 ? trimmedKey.Substring(0, 3) + "...REDACTED..." + trimmedKey.Substring(trimmedKey.Length - 3) : "[REDACTED]");
            // Add Accept header for JSON if not present
            if (!request.Headers.Contains("Accept"))
                request.Headers.Add("Accept", "application/json");
            // Log full request URL and headers for debugging
            var baseUrl = _httpClient.BaseAddress?.ToString() ?? string.Empty;
            var reqUri = request.RequestUri?.ToString() ?? string.Empty;
            _logger.LogDebug("[SendAsync] Request URL: {Url}", baseUrl + reqUri);
            foreach (var header in request.Headers)
            {
                if (header.Key.ToLower() == "authorization")
                    _logger.LogDebug("[SendAsync] Header: {Key} = [REDACTED]", header.Key);
                else
                    _logger.LogDebug("[SendAsync] Header: {Key} = {Value}", header.Key, string.Join(",", header.Value));
            }
            int retries = 0;
            TimeSpan delay = _initialDelay;
            while (true)
            {
                try
                {
                    var response = await _httpClient.SendAsync(request);
                    LogRequestAndResponse(request, response);

                    if (response.StatusCode == HttpStatusCode.NotFound)
                    {
                        _logger.LogWarning("404 Not Found: {Url}", request.RequestUri);
                        return response; // Graceful handling
                    }
                    if (response.StatusCode == (HttpStatusCode)429)
                    {
                        if (retries < _maxRetries)
                        {
                            _logger.LogWarning("429 Too Many Requests: Retrying after {Delay}s", delay.TotalSeconds);
                            await Task.Delay(delay);
                            delay = TimeSpan.FromSeconds(delay.TotalSeconds * 2); // Exponential backoff
                            retries++;
                            continue;
                        }
                        _logger.LogError("429 Too Many Requests: Max retries exceeded for {Url}", request.RequestUri);
                    }
                    return response;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error during Portnox API call to {Url}", request.RequestUri);
                    throw;
                }
            }
        }

        private void LogRequestAndResponse(HttpRequestMessage request, HttpResponseMessage response)
        {
            // Redact sensitive headers
            var headers = request.Headers.ToString().Replace(_apiKey, "[REDACTED]");
            string? body = null;
            if (request.Content != null)
            {
                // Only log if content is text-based (e.g., JSON, XML, plain text)
                var contentType = request.Content.Headers.ContentType?.MediaType;
                if (contentType != null && (contentType.Contains("json") || contentType.Contains("xml") || contentType.Contains("text")))
                {
                    try
                    {
                        body = request.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                    }
                    catch (Exception ex)
                    {
                        body = $"[Error reading body: {ex.Message}]";
                    }
                }
                else if (request.Content.Headers.ContentLength > 0)
                {
                    body = "[Non-text content not logged]";
                }
            }
            _logger.LogInformation("HTTP {Method} {Url}\nHeaders: {Headers}\nBody: {Body}\nStatus: {Status}",
                request.Method, request.RequestUri, headers, body, response.StatusCode);
        }
    }
}
