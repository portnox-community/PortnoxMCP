
using ModelContextProtocol.Server;
using System.ComponentModel;

namespace PortnoxMCP.Tools
{
    [McpServerToolType]
    public sealed class Tools
    {
        private readonly GetPortnoxSite _getPortnoxSite;
        private readonly GetPortnoxMACAccounts _getPortnoxMACAccounts;
        private readonly GetPortnoxDevices _getPortnoxDevices;

        public Tools(GetPortnoxSite getPortnoxSite, GetPortnoxMACAccounts getPortnoxMACAccounts, GetPortnoxDevices getPortnoxDevices)
        {
            _getPortnoxSite = getPortnoxSite;
            _getPortnoxMACAccounts = getPortnoxMACAccounts;
            _getPortnoxDevices = getPortnoxDevices;
        }

    [McpServerTool(Title = "list_sites")]
    [Description("Retrieves all sites from the Portnox API. Supports filtering by site name.")]
    public async Task<List<object>> ListSites(string? name = null)
        => await _getPortnoxSite.GetSitesAsync(name);

    [McpServerTool(Title = "list_mac_accounts")]
    [Description("Retrieves all MAC-based accounts from the Portnox API. Supports filtering by account name.")]
    public async Task<List<object>> ListMacAccounts(string? name = null)
        => await _getPortnoxMACAccounts.GetMACAccountsAsync(name);

    [McpServerTool(Title = "list_devices")]
    [Description("Retrieves devices from the Portnox API. Supports filtering by deviceId or deviceName.")]
    public async Task<List<object>> ListDevices(
        IMcpServer server,
        RequestContext<ModelContextProtocol.Protocol.CallToolRequestParams> context,
        string? deviceId = null,
        string? deviceName = null,
        int pageNumber = 1,
        int pageSize = 5,
        string? searchValue = null,
        int? searchField = null,
        int? clientTimeOffset = null,
        bool? includeAccountWithoutDevices = null,
        double? startTimeLimit = null,
        double? endTimeLimit = null,
        string? startReportedTimeLimit = null,
        string? endReportedTimeLimit = null)
        => await _getPortnoxDevices.GetDevicesAsync(
            server, context, deviceId, deviceName, pageNumber, pageSize, searchValue, searchField, clientTimeOffset, includeAccountWithoutDevices, startTimeLimit, endTimeLimit, startReportedTimeLimit, endReportedTimeLimit);
    }
}
