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
        [McpServerTool(
            Title = "get_portnox_sites",
            ReadOnly = true,
            Idempotent = true,
            Destructive = false
        )]
        [Description("Retrieves all sites from the Portnox API. Supports filtering by site name.")]
    public async Task<List<object>> GetSitesAsync(string? name = null)
        {
            _logger.LogDebug("[GetSitesAsync] Invoked with name={Name}", name);
            var req = new HttpRequestMessage(HttpMethod.Get, "/api/nases/sites");
            var resp = await _client.SendAsync(req);
            var json = await resp.Content.ReadAsStringAsync();
            var doc = JsonDocument.Parse(json);
            var sites = new List<object>();
            if (doc.RootElement.TryGetProperty("sites", out var sitesArr))
            {
                foreach (var site in sitesArr.EnumerateArray())
                    sites.Add(site);
            }
            if (!string.IsNullOrEmpty(name))
            {
                _logger.LogDebug("[GetSitesAsync] Filtering sites by name: {Name}", name);
                sites = sites.FindAll(s => s.ToString()!.Contains(name!));
            }
            _logger.LogDebug("[GetSitesAsync] Returning {Count} sites", sites.Count);
            return sites;
        }
    }
}
