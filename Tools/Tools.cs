
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
    [Description("Retrieves devices from the Portnox API. Supports filtering by deviceId or any individual search field.")]
    public async Task<List<object>> ListDevices(
    IMcpServer server,
    RequestContext<ModelContextProtocol.Protocol.CallToolRequestParams> context,
    int pageNumber,
    int pageSize,
    string? deviceId = null,
    string? id = null,
    string? deviceIdField = null,
        string? orgId = null,
        string? statusField = null,
        string? orgPresence = null,
        string? account = null,
        string? accountName_UI = null,
        string? osName = null,
        string? risk = null,
        string? geoInfo = null,
        string? ipAdresses = null,
        string? macAdresses = null,
        string? applications = null,
        string? hotfixes = null,
        string? loggedUsers = null,
        string? processes = null,
        string? browser = null,
        string? services = null,
        string? ssid = null,
        string? browserExtension = null,
        string? certificates = null,
        string? groupName = null,
        string? macVendor = null,
        string? manufacturer = null,
        string? network = null,
        string? peripheral = null,
        string? criticalSoftware = null,
        string? riskScore = null,
        string? location = null,
        string? formFactor = null,
        string? nasIp = null,
        string? nasType = null,
        string? nasId = null,
        string? agentPVersion = null,
        string? panwThreatName = null,
        string? panwThreatCategory = null,
        string? accountType = null,
        string? lastConnected = null,
        string? lastConnectedScore = null,
        string? authenticationRepositoryType = null,
        string? model = null,
        string? accountContainsDevices = null,
        string? deviceName = null,
        string? siteFullPath = null,
        string? lastReportedTime = null,
        string? accountAlias = null,
        string? geographyPoint = null,
        int? clientTimeOffset = null,
    bool? includeAccountWithoutDevices = null
    )
        => await _getPortnoxDevices.GetDevicesAsync(
            server, context, pageNumber, pageSize, deviceId, id, deviceIdField, orgId, statusField, orgPresence, account, accountName_UI, osName, risk, geoInfo, ipAdresses, macAdresses, applications, hotfixes, loggedUsers, processes, browser, services, ssid, browserExtension, certificates, groupName, macVendor, manufacturer, network, peripheral, criticalSoftware, riskScore, location, formFactor, nasIp, nasType, nasId, agentPVersion, panwThreatName, panwThreatCategory, accountType, lastConnected, lastConnectedScore, authenticationRepositoryType, model, accountContainsDevices, deviceName, siteFullPath, lastReportedTime, accountAlias, geographyPoint, clientTimeOffset, includeAccountWithoutDevices);
    }
}
