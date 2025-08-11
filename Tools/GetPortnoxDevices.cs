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
    public async Task<List<object>> GetDevicesAsync(
    IMcpServer server,
    RequestContext<CallToolRequestParams> context,
    string? deviceId = null,
    string? deviceName = null,
    int? pageNumber = null, // Requested page number
    int? pageSize = null, // Requested page size (1-30)
    string? searchValue = null, // Value of parameter to search by
    int? searchField = null, // Searching field (see DeviceSearchRequest enum)
    int? clientTimeOffset = null, // Timezone offset
    bool? includeAccountWithoutDevices = null, // By default all accounts are returned. Set to false to return only accounts with at least one device.
    double? startTimeLimit = null, // Start time limit (double)
    double? endTimeLimit = null, // End time limit (double)
    string? startReportedTimeLimit = null, // Start reported time limit (date-time string)
    string? endReportedTimeLimit = null // End reported time limit (date-time string)
    )
        {
            // Clamp pageSize to [1, 30] as per API docs
            int effectivePageSize = pageSize ?? 30;
            if (effectivePageSize < 1) effectivePageSize = 1;
            if (effectivePageSize > 30) effectivePageSize = 30;
            int effectivePageNumber = pageNumber ?? 1;
            _logger.LogDebug("[GetDevicesAsync] Invoked with deviceId={DeviceId}, deviceName={DeviceName}, pageNumber={PageNumber}, pageSize={PageSize}, searchValue={SearchValue}, searchField={SearchField}", deviceId, deviceName, effectivePageNumber, effectivePageSize, searchValue, searchField);
            var devices = new List<object>();
            var progressToken = context.Params?.ProgressToken;
            int progress = 0;
            var baseUrl = "https://clear.portnox.com:8081/CloudPortalBackEnd/";

            if (!string.IsNullOrEmpty(deviceId))
            {
                var path = $"api/device/{deviceId}";
                var req = new HttpRequestMessage(HttpMethod.Get, new Uri(baseUrl + path));
                _logger.LogDebug("[GetDevicesAsync] Fetching device by ID: {DeviceId} | Full URL: {FullUrl}", deviceId, req.RequestUri);
                var resp = await _client.SendAsync(req);
                var status = (int)resp.StatusCode;
                var json = await resp.Content.ReadAsStringAsync();
                _logger.LogDebug("[GetDevicesAsync] Response for deviceId {DeviceId}: Status={Status}, Headers={Headers}, Body={Json}", deviceId, status, resp.Headers, json);
                if (status != 200)
                {
                    _logger.LogError("[GetDevicesAsync] Non-200 response for deviceId {DeviceId}: {Json}", deviceId, json);
                    return new List<object>();
                }
                // Try to parse as object or array
                using var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;
                if (root.ValueKind == JsonValueKind.Object)
                {
                    devices.Add(JsonElementToObject(root));
                }
                else if (root.ValueKind == JsonValueKind.Array)
                {
                    foreach (var entry in root.EnumerateArray())
                        devices.Add(JsonElementToObject(entry));
                }
                else
                {
                    _logger.LogWarning("[GetDevicesAsync] Unexpected JSON type for deviceId response: {Type}", root.ValueKind);
                }
                _logger.LogDebug("[GetDevicesAsync] Returning device(s) for ID: {DeviceId}, count: {Count}", deviceId, devices.Count);
                // Send progress notification for single device fetch
                if (progressToken is not null)
                {
                    await server.SendNotificationAsync("notifications/progress", new { Progress = 1, Total = 1, progressToken });
                }
                return devices;
            }

            int pageIdx = 1;
            bool firstPage = true;
            int totalPages = 1000; // Arbitrary large number, will be corrected after first page
            while (true)
            {
                var path = "api/device/list";
                // Build DeviceQueryRequest and DeviceSearchRequest objects
                var queryObj = new Dictionary<string, object?>
                {
                    ["PageNumber"] = pageNumber ?? pageIdx,
                    ["PageSize"] = effectivePageSize
                };
                if (clientTimeOffset.HasValue) queryObj["ClientTimeOffset"] = clientTimeOffset.Value;
                if (includeAccountWithoutDevices.HasValue) queryObj["IncludeAccountWithoutDevices"] = includeAccountWithoutDevices.Value;
                if (startTimeLimit.HasValue) queryObj["StartTimeLimit"] = startTimeLimit.Value;
                if (endTimeLimit.HasValue) queryObj["EndTimeLimit"] = endTimeLimit.Value;
                if (!string.IsNullOrEmpty(startReportedTimeLimit)) queryObj["StartReportedTimeLimit"] = startReportedTimeLimit;
                if (!string.IsNullOrEmpty(endReportedTimeLimit)) queryObj["EndReportedTimeLimit"] = endReportedTimeLimit;

                Dictionary<string, object?>? searchObj = null;
                if (!string.IsNullOrEmpty(deviceName) || !string.IsNullOrEmpty(searchValue) || searchField.HasValue)
                {
                    searchObj = new Dictionary<string, object?>();
                    if (!string.IsNullOrEmpty(deviceName))
                    {
                        searchObj["Value"] = deviceName;
                        searchObj["Field"] = 42; // DeviceName enum
                    }
                    else if (!string.IsNullOrEmpty(searchValue) && searchField.HasValue)
                    {
                        searchObj["Value"] = searchValue;
                        searchObj["Field"] = searchField.Value;
                    }
                    else if (!string.IsNullOrEmpty(searchValue))
                    {
                        searchObj["Value"] = searchValue;
                    }
                    else if (searchField.HasValue)
                    {
                        searchObj["Field"] = searchField.Value;
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
                _logger.LogDebug("[GetDevicesAsync] Fetching device page {PageIdx} | Full URL: {FullUrl}", pageIdx, req.RequestUri);
                var resp = await _client.SendAsync(req);
                var status = (int)resp.StatusCode;
                var json = await resp.Content.ReadAsStringAsync();
                if (firstPage)
                {
                    _logger.LogInformation("[GetDevicesAsync] RAW JSON response for first page: {Json}", json);
                    firstPage = false;
                }
                _logger.LogDebug("[GetDevicesAsync] Response for page {PageIdx}: Status={Status}, Headers={Headers}, Body={Json}", pageIdx, status, resp.Headers, json);
                if (status != 200)
                {
                    _logger.LogError("[GetDevicesAsync] Non-200 response for page {PageIdx}: {Json}", pageIdx, json);
                    break;
                }
                using var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;

                // Handle if root is an array of devices
                if (root.ValueKind == JsonValueKind.Array)
                {
                    int count = 0;
                    foreach (var entry in root.EnumerateArray())
                    {
                        devices.Add(JsonElementToObject(entry));
                        count++;
                        progress++;
                        if (progressToken is not null)
                        {
                            await server.SendNotificationAsync("notifications/progress", new { Progress = progress, Total = totalPages, progressToken });
                        }
                    }
                    _logger.LogInformation("[GetDevicesAsync] Devices extracted from root array: {Count}", count);
                    break;
                }
                // Handle if root is a single device object or a container
                else if (root.ValueKind == JsonValueKind.Object)
                {
                    // Try to find 'Result' property
                    if (root.TryGetProperty("Result", out var result) && result.ValueKind != JsonValueKind.Null)
                    {
                        // If Result is an array (array of objects with Devices array)
                        if (result.ValueKind == JsonValueKind.Array)
                        {
                            int total = 0;
                            foreach (var resultEntry in result.EnumerateArray())
                            {
                                if (resultEntry.ValueKind == JsonValueKind.Object && resultEntry.TryGetProperty("Devices", out var devicesArr) && devicesArr.ValueKind == JsonValueKind.Array)
                                {
                                    int count = 0;
                                    foreach (var device in devicesArr.EnumerateArray())
                                    {
                                        devices.Add(JsonElementToObject(device));
                                        count++;
                                        total++;
                                        progress++;
                                        if (progressToken is not null)
                                        {
                                            await server.SendNotificationAsync("notifications/progress", new { Progress = progress, Total = totalPages, progressToken });
                                        }
                                    }
                                    _logger.LogInformation("[GetDevicesAsync] Devices extracted from Result array entry: {Count}", count);
                                }
                                else
                                {
                                    _logger.LogWarning("[GetDevicesAsync] No 'Devices' array found in Result array entry: {Entry}", resultEntry.ToString());
                                }
                            }
                            _logger.LogInformation("[GetDevicesAsync] Total devices extracted from Result array: {Total}", total);
                            break;
                        }
                        // If Result is an object with 'devices' array
                        else if (result.ValueKind == JsonValueKind.Object)
                        {
                            if (result.TryGetProperty("devices", out var devicesArr) && devicesArr.ValueKind == JsonValueKind.Array)
                            {
                                int count = 0;
                                foreach (var entry in devicesArr.EnumerateArray())
                                {
                                    devices.Add(JsonElementToObject(entry));
                                    count++;
                                    progress++;
                                    if (progressToken is not null)
                                    {
                                        await server.SendNotificationAsync("notifications/progress", new { Progress = progress, Total = totalPages, progressToken });
                                    }
                                }
                                _logger.LogInformation("[GetDevicesAsync] Devices extracted from Result.devices array: {Count}", count);
                                pageIdx++;
                                continue;
                            }
                            else
                            {
                                _logger.LogWarning("[GetDevicesAsync] 'devices' property is not an array or missing in Result at page {PageIdx}. Value: {Value}", pageIdx, result.ToString());
                            }
                        }
                        else
                        {
                            _logger.LogWarning("[GetDevicesAsync] Result property is not an object or array at page {PageIdx}. Value: {Value}", pageIdx, result.ToString());
                        }
                    }
                    else
                    {
                        // If no 'Result', treat root as a device object or a container
                        _logger.LogInformation("[GetDevicesAsync] Root object does not have 'Result'. Treating as device or container.");
                        devices.Add(JsonElementToObject(root));
                    }
                    break;
                }
                else
                {
                    _logger.LogError("[GetDevicesAsync] Unexpected JSON root element type: {Type}", root.ValueKind);
                    break;
                }
            }

            _logger.LogInformation("[GetDevicesAsync] Final device list count: {Count}", devices.Count);
            if (devices.Count > 0)
                _logger.LogInformation("[GetDevicesAsync] Sample device: {Sample}", JsonSerializer.Serialize(devices[0]));
            else
                _logger.LogWarning("[GetDevicesAsync] Device list is empty at return.");
            return devices ?? new List<object>();
        }
    }
}
