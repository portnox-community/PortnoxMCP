
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
    public async Task<List<GetPortnoxSite.SiteInfo>> ListSites(string? name = null)
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
    [Description("Page number for pagination (1-based). Determines which page of results to return.")]
    int pageNumber,
    [Description("Number of devices per page (max 100). Determines the size of each result page.")]
    int pageSize,
    [Description("Unique device ID to fetch a specific device. If provided, all other filters are ignored.")]
    string? deviceId = null,
    [Description("Device unique identifier for search filtering.")]
    string? id = null,
    [Description("Device ID field for search filtering.")]
    string? deviceIdField = null,
    [Description("Organization ID for filtering devices by organization.")]
    string? orgId = null,
    [Description("Device status field for search filtering.")]
    string? statusField = null,
    [Description("Organization presence filter (e.g., present/absent).")]
    string? orgPresence = null,
    [Description("Account name or ID for filtering devices by account.")]
    string? account = null,
    [Description("UI display name of the account for filtering.")]
    string? accountName_UI = null,
    [Description("Operating system name for filtering devices.")]
    string? osName = null,
    [Description("Device risk level for filtering.")]
    string? risk = null,
    [Description("Geographical information for device filtering.")]
    string? geoInfo = null,
    [Description("IP addresses for filtering devices (comma-separated).")]
    string? ipAdresses = null,
    [Description("MAC addresses for filtering devices (comma-separated).")]
    string? macAdresses = null,
    [Description("Installed applications for filtering devices.")]
    string? applications = null,
    [Description("Installed hotfixes for filtering devices.")]
    string? hotfixes = null,
    [Description("Logged-in users for filtering devices.")]
    string? loggedUsers = null,
    [Description("Running processes for filtering devices.")]
    string? processes = null,
    [Description("Browser name/version for filtering devices.")]
    string? browser = null,
    [Description("Services running on the device for filtering.")]
    string? services = null,
    [Description("SSID (Wi-Fi network) for filtering devices.")]
    string? ssid = null,
    [Description("Browser extension for filtering devices.")]
    string? browserExtension = null,
    [Description("Installed certificates for filtering devices.")]
    string? certificates = null,
    [Description("Group name for filtering devices.")]
    string? groupName = null,
    [Description("MAC vendor for filtering devices.")]
    string? macVendor = null,
    [Description("Device manufacturer for filtering.")]
    string? manufacturer = null,
    [Description("Network name or ID for filtering devices.")]
    string? network = null,
    [Description("Peripheral devices for filtering.")]
    string? peripheral = null,
    [Description("Critical software for filtering devices.")]
    string? criticalSoftware = null,
    [Description("Device risk score for filtering.")]
    string? riskScore = null,
    [Description("Device location for filtering.")]
    string? location = null,
    [Description("Device form factor (e.g., laptop, desktop) for filtering.")]
    string? formFactor = null,
    [Description("NAS (Network Attached Storage) IP address for filtering.")]
    string? nasIp = null,
    [Description("NAS type for filtering devices.")]
    string? nasType = null,
    [Description("NAS ID for filtering devices.")]
    string? nasId = null,
    [Description("Agent P version for filtering devices.")]
    string? agentPVersion = null,
    [Description("Palo Alto Networks threat name for filtering devices.")]
    string? panwThreatName = null,
    [Description("Palo Alto Networks threat category for filtering devices.")]
    string? panwThreatCategory = null,
    [Description("Account type for filtering devices.")]
    string? accountType = null,
    [Description("Last connected time for filtering devices.")]
    string? lastConnected = null,
    [Description("Last connected score for filtering devices.")]
    string? lastConnectedScore = null,
    [Description("Authentication repository type for filtering devices.")]
    string? authenticationRepositoryType = null,
    [Description("Device model for filtering.")]
    string? model = null,
    [Description("Whether the account contains devices (true/false) for filtering.")]
    string? accountContainsDevices = null,
    [Description("Device name for filtering.")]
    string? deviceName = null,
    [Description("Full site path for filtering devices.")]
    string? siteFullPath = null,
    [Description("Last reported time for filtering devices.")]
    string? lastReportedTime = null,
    [Description("Account alias for filtering devices.")]
    string? accountAlias = null,
    [Description("Geography point (latitude/longitude) for filtering devices.")]
    string? geographyPoint = null,
    [Description("Client timezone offset in minutes (for time-based filtering).")]
    int? clientTimeOffset = null,
    [Description("If true, include accounts without devices; if false, only accounts with at least one device are returned.")]
    bool? includeAccountWithoutDevices = null
    )
        => await _getPortnoxDevices.GetDevicesAsync(
            server, context, pageNumber, pageSize, deviceId, id, deviceIdField, orgId, statusField, orgPresence, account, accountName_UI, osName, risk, geoInfo, ipAdresses, macAdresses, applications, hotfixes, loggedUsers, processes, browser, services, ssid, browserExtension, certificates, groupName, macVendor, manufacturer, network, peripheral, criticalSoftware, riskScore, location, formFactor, nasIp, nasType, nasId, agentPVersion, panwThreatName, panwThreatCategory, accountType, lastConnected, lastConnectedScore, authenticationRepositoryType, model, accountContainsDevices, deviceName, siteFullPath, lastReportedTime, accountAlias, geographyPoint, clientTimeOffset, includeAccountWithoutDevices);
    }
}
