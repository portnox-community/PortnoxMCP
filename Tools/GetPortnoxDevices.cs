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
            int? clientTimeOffset = null, // Timezone offset
            bool? includeAccountWithoutDevices = null // By default all accounts are returned. Set to false to return only accounts with at least one device.
        )
        {
            // Clamp pageSize to [1, 30] as per API docs
            int effectivePageSize = pageSize;
            if (effectivePageSize < 1) effectivePageSize = 1;
            if (effectivePageSize > 30) effectivePageSize = 30;
            int effectivePageNumber = pageNumber;
            _logger.LogDebug("[GetDevicesAsync] Invoked with deviceId={DeviceId}, pageNumber={PageNumber}, pageSize={PageSize}", deviceId, effectivePageNumber, effectivePageSize);
            var devices = new List<object>();
            var progressToken = context.Params?.ProgressToken;
            int progress = 0;
            var baseUrl = "https://clear.portnox.com:8081/CloudPortalBackEnd/";

            if (!string.IsNullOrEmpty(deviceId))
            {
                var devicePath = $"api/device/{deviceId}";
                var deviceReq = new HttpRequestMessage(HttpMethod.Get, new Uri(baseUrl + devicePath));
                _logger.LogDebug("[GetDevicesAsync] Fetching device by ID: {DeviceId} | Full URL: {FullUrl}", deviceId, deviceReq.RequestUri);
                var deviceResp = await _client.SendAsync(deviceReq);
                var deviceStatus = (int)deviceResp.StatusCode;
                var deviceJson = await deviceResp.Content.ReadAsStringAsync();
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
            _logger.LogDebug("[GetDevicesAsync] Fetching device page {PageNumber} | Full URL: {FullUrl}", pageNumber, req.RequestUri);
            var resp = await _client.SendAsync(req);
            var status = (int)resp.StatusCode;
            var json = await resp.Content.ReadAsStringAsync();
            _logger.LogInformation("[GetDevicesAsync] RAW JSON response for page {PageNumber}: {Json}", pageNumber, json);
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
                _logger.LogInformation("[GetDevicesAsync] Sample device: {Sample}", JsonSerializer.Serialize(devices[0]));
            else
                _logger.LogWarning("[GetDevicesAsync] Device list is empty at return.");
            // Ensure all code paths return a value
            return devices ?? new List<object>();
        }
        }
        }
