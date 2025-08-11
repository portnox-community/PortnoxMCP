using ModelContextProtocol;
using ModelContextProtocol.Protocol;
using ModelContextProtocol.Server;
using System.ComponentModel;
using ModelContextProtocol.AspNetCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;
using PortnoxMCP.Tools; // For DeviceSearchField

namespace PortnoxMCP.Tools
{

    [McpServerToolType]
    public sealed class GetPortnoxDevices
    {
        private readonly PortnoxApiClient _client;
        private readonly ILogger<GetPortnoxDevices> _logger;

        // Recursively convert a JsonElement to a plain .NET object
        private static object JsonElementToObject(JsonElement element)
        {
            switch (element.ValueKind)
            {
                case JsonValueKind.Object:
                    var dict = new Dictionary<string, object>();
                    foreach (var prop in element.EnumerateObject())
                        dict[prop.Name] = JsonElementToObject(prop.Value);
                    return dict;
                case JsonValueKind.Array:
                    var list = new List<object>();
                    foreach (var item in element.EnumerateArray())
                        list.Add(JsonElementToObject(item));
                    return list;
                case JsonValueKind.String:
                    return element.GetString() ?? string.Empty;
                case JsonValueKind.Number:
                    if (element.TryGetInt64(out long l)) return l;
                    if (element.TryGetDouble(out double d)) return d;
                    return element.GetRawText();
                case JsonValueKind.True:
                case JsonValueKind.False:
                    return element.GetBoolean();
                case JsonValueKind.Null:
                case JsonValueKind.Undefined:
                    return string.Empty;
                default:
                    return element.GetRawText();
            }
        }

        public GetPortnoxDevices(PortnoxApiClient client, ILogger<GetPortnoxDevices> logger)
        {
            _client = client;
            _logger = logger;
        }

        /// <summary>
        /// Retrieves devices from the Portnox API. Supports all DeviceQueryRequest and DeviceSearchRequest fields as input.
        /// Handles pagination automatically. Accepts pageNumber, pageSize, searchValue, searchField, clientTimeOffset, includeAccountWithoutDevices, time limits, and more.
        /// </summary>
        [McpServerTool(
            Title = "get_portnox_devices",
            ReadOnly = true,
            Idempotent = true,
            Destructive = false
        )]
        [Description("Retrieves devices from the Portnox API. Supports all DeviceQueryRequest and DeviceSearchRequest fields as input. Handles pagination automatically. Accepts pageNumber, pageSize, searchValue, searchField, clientTimeOffset, includeAccountWithoutDevices, time limits, and more.")]
    /// <summary>
    /// Retrieves devices from the Portnox API. Supports all DeviceQueryRequest and DeviceSearchRequest fields as input.
    /// Handles pagination automatically. Accepts pageNumber, pageSize, searchField (as enum), searchValue, clientTimeOffset, includeAccountWithoutDevices, time limits, and more.
    /// </summary>
        public async Task<List<object>> GetDevicesAsync(
            IMcpServer server,
            RequestContext<CallToolRequestParams> context,
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
        {
            int effectivePageSize = pageSize;
            if (effectivePageSize < 1) effectivePageSize = 1;
            if (effectivePageSize > 100) effectivePageSize = 100;
            int effectivePageNumber = pageNumber;
            if (_logger.IsEnabled(LogLevel.Debug))
                _logger.LogDebug("[GetDevicesAsync] Invoked with deviceId={DeviceId}, pageNumber={PageNumber}, pageSize={PageSize}", deviceId, effectivePageNumber, effectivePageSize);
            var devices = new List<object>();
            var progressToken = context.Params?.ProgressToken;
            var baseUrl = "https://clear.portnox.com:8081/CloudPortalBackEnd/";

            if (!string.IsNullOrEmpty(deviceId))
            {
                var devicePath = $"api/device/{deviceId}";
                var deviceReq = new HttpRequestMessage(HttpMethod.Get, new Uri(baseUrl + devicePath));
                if (_logger.IsEnabled(LogLevel.Debug))
                    _logger.LogDebug("[GetDevicesAsync] Fetching device by ID: {DeviceId} | Full URL: {FullUrl}", deviceId, deviceReq.RequestUri);
                var deviceResp = await _client.SendAsync(deviceReq);
                var deviceStatus = (int)deviceResp.StatusCode;
                var deviceJson = await deviceResp.Content.ReadAsStringAsync();
                if (_logger.IsEnabled(LogLevel.Debug))
                    _logger.LogDebug("[GetDevicesAsync] Response for deviceId {DeviceId}: Status={Status}, Headers={Headers}, Body={Json}", deviceId, deviceStatus, deviceResp.Headers, deviceJson);
                if (deviceStatus != 200)
                {
                    _logger.LogError("[GetDevicesAsync] Non-200 response for deviceId {DeviceId}: {Json}", deviceId, deviceJson);
                    return new List<object>();
                }
                // Try to parse as object or array
                using var deviceDoc = JsonDocument.Parse(deviceJson);
                var deviceRoot = deviceDoc.RootElement;
                if (deviceRoot.ValueKind == JsonValueKind.Object)
                {
                    devices.Add(JsonElementToObject(deviceRoot));
                }
                else if (deviceRoot.ValueKind == JsonValueKind.Array)
                {
                    foreach (var entry in deviceRoot.EnumerateArray())
                        devices.Add(JsonElementToObject(entry));
                }
                else
                {
                    _logger.LogWarning("[GetDevicesAsync] Unexpected JSON type for deviceId response: {Type}", deviceRoot.ValueKind);
                }
                if (_logger.IsEnabled(LogLevel.Debug))
                    _logger.LogDebug("[GetDevicesAsync] Returning device(s) for ID: {DeviceId}, count: {Count}", deviceId, devices.Count);
                // Send progress notification for single device fetch
                if (progressToken is not null)
                {
                    await server.SendNotificationAsync("notifications/progress", new { Progress = 1, Total = 1, progressToken });
                }
                return devices;
            }

            // Only fetch the requested page once (no auto-pagination)
            var path = "api/device/list";
            var queryObj = new Dictionary<string, object?>
            {
                ["PageNumber"] = pageNumber,
                ["PageSize"] = effectivePageSize
            };
            if (clientTimeOffset.HasValue) queryObj["ClientTimeOffset"] = clientTimeOffset.Value;
            if (includeAccountWithoutDevices.HasValue) queryObj["IncludeAccountWithoutDevices"] = includeAccountWithoutDevices.Value;
            // time limit parameters removed

            // Map field names to DeviceSearchField enum values
            var searchFields = new (string? value, DeviceSearchField field)[]
            {
                (id, DeviceSearchField.Id),
                (deviceIdField, DeviceSearchField.DeviceId),
                (orgId, DeviceSearchField.OrgId),
                (statusField, DeviceSearchField.Status),
                (orgPresence, DeviceSearchField.OrgPresence),
                (account, DeviceSearchField.Account),
                (accountName_UI, DeviceSearchField.AccountName_UI),
                (osName, DeviceSearchField.OsName),
                (risk, DeviceSearchField.Risk),
                (geoInfo, DeviceSearchField.GeoInfo),
                (ipAdresses, DeviceSearchField.IpAdresses),
                (macAdresses, DeviceSearchField.MacAdresses),
                (applications, DeviceSearchField.Applications),
                (hotfixes, DeviceSearchField.Hotfixes),
                (loggedUsers, DeviceSearchField.LoggedUsers),
                (processes, DeviceSearchField.Processes),
                (browser, DeviceSearchField.Browser),
                (services, DeviceSearchField.Services),
                (ssid, DeviceSearchField.Ssid),
                (browserExtension, DeviceSearchField.BrowserExtension),
                (certificates, DeviceSearchField.Certificates),
                (groupName, DeviceSearchField.GroupName),
                (macVendor, DeviceSearchField.MacVendor),
                (manufacturer, DeviceSearchField.Manufacturer),
                (network, DeviceSearchField.Network),
                (peripheral, DeviceSearchField.Peripheral),
                (criticalSoftware, DeviceSearchField.CriticalSoftware),
                (riskScore, DeviceSearchField.RiskScore),
                (location, DeviceSearchField.Location),
                (formFactor, DeviceSearchField.FormFactor),
                (nasIp, DeviceSearchField.NasIp),
                (nasType, DeviceSearchField.NasType),
                (nasId, DeviceSearchField.NasId),
                (agentPVersion, DeviceSearchField.AgentPVersion),
                (panwThreatName, DeviceSearchField.PanwThreatName),
                (panwThreatCategory, DeviceSearchField.PanwThreatCategory),
                (accountType, DeviceSearchField.AccountType),
                (lastConnected, DeviceSearchField.LastConnected),
                (lastConnectedScore, DeviceSearchField.LastConnectedScore),
                (authenticationRepositoryType, DeviceSearchField.AuthenticationRepositoryType),
                (model, DeviceSearchField.Model),
                (accountContainsDevices, DeviceSearchField.AccountContainsDevices),
                (deviceName, DeviceSearchField.DeviceName),
                (siteFullPath, DeviceSearchField.SiteFullPath),
                (lastReportedTime, DeviceSearchField.LastReportedTime),
                (accountAlias, DeviceSearchField.AccountAlias),
                (geographyPoint, DeviceSearchField.GeographyPoint)
            };

            Dictionary<string, object?>? searchObj = null;
            foreach (var (value, field) in searchFields)
            {
                if (!string.IsNullOrEmpty(value))
                {
                    searchObj = new Dictionary<string, object?>
                    {
                        ["Field"] = (int)field,
                        ["Value"] = value
                    };
                    break; // Only use the first non-null field
                }
            }

            var bodyDict = new Dictionary<string, object?>
            {
                ["Query"] = queryObj
            };
            if (searchObj != null)
                bodyDict["Search"] = searchObj;

            var req = new HttpRequestMessage(HttpMethod.Post, new Uri(baseUrl + path))
            {
                Content = new StringContent(JsonSerializer.Serialize(bodyDict), System.Text.Encoding.UTF8, "application/json")
            };
            if (_logger.IsEnabled(LogLevel.Debug))
                _logger.LogDebug("[GetDevicesAsync] Fetching device page {PageNumber} | Full URL: {FullUrl}", pageNumber, req.RequestUri);
            var resp = await _client.SendAsync(req);
            var status = (int)resp.StatusCode;
            var json = await resp.Content.ReadAsStringAsync();
            if (_logger.IsEnabled(LogLevel.Debug))
                _logger.LogDebug("[GetDevicesAsync] RAW JSON response for page {PageNumber}: {Json}", pageNumber, json);
            if (_logger.IsEnabled(LogLevel.Debug))
                _logger.LogDebug("[GetDevicesAsync] Response for page {PageNumber}: Status={Status}, Headers={Headers}, Body={Json}", pageNumber, status, resp.Headers, json);
            if (status != 200)
            {
                _logger.LogError("[GetDevicesAsync] Non-200 response for page {PageNumber}: {Json}", pageNumber, json);
            }
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            // Extract the "Result" array and add each item to the devices list
            if (root.TryGetProperty("Result", out var resultElement) && resultElement.ValueKind == JsonValueKind.Array)
            {
                foreach (var item in resultElement.EnumerateArray())
                {
                    devices.Add(JsonElementToObject(item));
                }
            }
            else
            {
                _logger.LogWarning("[GetDevicesAsync] No 'Result' array found in response JSON.");
            }

            _logger.LogInformation("[GetDevicesAsync] Final device list count: {Count}", devices.Count);
            if (devices.Count > 0)
            {
                if (_logger.IsEnabled(LogLevel.Debug))
                    _logger.LogDebug("[GetDevicesAsync] Sample device: {Sample}", JsonSerializer.Serialize(devices[0]));
            }
            else
                _logger.LogWarning("[GetDevicesAsync] Device list is empty at return.");
            // Ensure all code paths return a value
            return devices ?? new List<object>();
        }
        }
        }
