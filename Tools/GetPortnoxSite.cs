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
    public sealed class GetPortnoxSite
    {
        private readonly PortnoxApiClient _client;
        private readonly ILogger<GetPortnoxSite> _logger;

        public GetPortnoxSite(PortnoxApiClient client, ILogger<GetPortnoxSite> logger)
        {
            _client = client;
            _logger = logger;
        }

        /// <summary>
        /// Retrieves all sites from the Portnox API. Supports filtering by name.
        /// </summary>
        public class SiteInfo
        {
            public string? Description { get; set; }
            public string? Id { get; set; }
            public string? Name { get; set; }
            public string? ParentId { get; set; }
            public object? Rules { get; set; }
        }

        [McpServerTool(
            Title = "get_portnox_sites",
            ReadOnly = true,
            Idempotent = true,
            Destructive = false
        )]
        [Description("Retrieves all sites from the Portnox API. Supports filtering by site name.")]
        public async Task<List<SiteInfo>> GetSitesAsync(string? name = null)
        {
            _logger.LogDebug("[GetSitesAsync] Invoked with name={Name}", name);
            var req = new HttpRequestMessage(HttpMethod.Get, "api/nases/sites")
            {
                Content = new StringContent("{}", System.Text.Encoding.UTF8, "application/json")
            };
            req.Headers.TryAddWithoutValidation("Content-Type", "application/json");
            var resp = await _client.SendAsync(req);
            var json = await resp.Content.ReadAsStringAsync();
            _logger.LogDebug("[GetSitesAsync] Raw JSON response: {Json}", json);
            var doc = JsonDocument.Parse(json);
            var sites = new List<SiteInfo>();
            if (doc.RootElement.ValueKind == JsonValueKind.Array)
            {
                foreach (var site in doc.RootElement.EnumerateArray())
                {
                    var siteInfo = JsonSerializer.Deserialize<SiteInfo>(site.GetRawText());
                    if (siteInfo != null) sites.Add(siteInfo);
                }
            }
            else if ((doc.RootElement.TryGetProperty("sites", out var sitesArr) && sitesArr.ValueKind == JsonValueKind.Array) ||
                     (doc.RootElement.TryGetProperty("Sites", out sitesArr) && sitesArr.ValueKind == JsonValueKind.Array))
            {
                foreach (var site in sitesArr.EnumerateArray())
                {
                    var siteInfo = JsonSerializer.Deserialize<SiteInfo>(site.GetRawText());
                    if (siteInfo != null) sites.Add(siteInfo);
                }
            }
            if (!string.IsNullOrEmpty(name))
            {
                _logger.LogDebug("[GetSitesAsync] Filtering sites by name: {Name}", name);
                sites = sites.FindAll(s => (s.Name != null && s.Name.Contains(name, System.StringComparison.OrdinalIgnoreCase)));
            }
            _logger.LogDebug("[GetSitesAsync] Returning {Count} sites", sites.Count);
            return sites;
        }
    }
}
