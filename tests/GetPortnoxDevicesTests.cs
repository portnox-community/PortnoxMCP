using System;
using System.Collections.Generic;
using System.Threading;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using PortnoxMCP.Tools;
using PortnoxMCP;

public class GetPortnoxDevicesTests
{
    private readonly Mock<PortnoxApiClient> _mockClient = new Mock<PortnoxApiClient>();
    private readonly Mock<ILogger<GetPortnoxDevices>> _mockLogger = new Mock<ILogger<GetPortnoxDevices>>();
    private readonly GetPortnoxDevices _service;

    public GetPortnoxDevicesTests()
    {
        _service = new GetPortnoxDevices(_mockClient.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task GetDevicesAsync_AppliesPageSizeLimits()
    {
        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("{\"Result\":[]}")
        };
    _mockClient.Setup(m => m.SendAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<bool>())).ReturnsAsync(response);

        await _service.GetDevicesAsync(null, null, 1, 200); // Should cap to 100
    // Avoid .Result and use async lambda for verification
    _mockClient.Verify(m => m.SendAsync(
        It.Is<HttpRequestMessage>(req => req.Content.ReadAsStringAsync().GetAwaiter().GetResult().Contains("\"PageSize\":100")),
        It.IsAny<bool>()
    ), Times.Once());
    }

    [Fact]
    public async Task GetDevicesAsync_ReturnsEmptyList_OnHttpRequestException()
    {
    _mockClient.Setup(m => m.SendAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<bool>())).ThrowsAsync(new HttpRequestException("Network error"));
        var result = await _service.GetDevicesAsync(null, null, 1, 10);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetDevicesAsync_ReturnsEmptyList_OnUnauthorized()
    {
        var response = new HttpResponseMessage(HttpStatusCode.Unauthorized)
        {
            Content = new StringContent("{}")
        };
    _mockClient.Setup(m => m.SendAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<bool>())).ReturnsAsync(response);
        var result = await _service.GetDevicesAsync(null, null, 1, 10);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetDevicesAsync_Handles429WithRetry()
    {
        int callCount = 0;
    _mockClient.Setup(m => m.SendAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<bool>())).ReturnsAsync(() =>
        {
            callCount++;
            if (callCount < 2)
                return new HttpResponseMessage((HttpStatusCode)429) { Content = new StringContent("{}") };
            return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("{\"Result\":[{\"DeviceId\":\"1\"}]} ") };
        });
        var result = await _service.GetDevicesAsync(null, null, 1, 10);
        Assert.Single(result);
        Assert.Equal(2, callCount);
    }

    [Fact]
    public async Task GetDevicesAsync_LogsError_OnException()
    {
    _mockClient.Setup(m => m.SendAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<bool>())).ThrowsAsync(new HttpRequestException("fail"));
        await _service.GetDevicesAsync(null, null, 1, 10);
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("fail")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()
            ), Times.AtLeastOnce);
    }
}
