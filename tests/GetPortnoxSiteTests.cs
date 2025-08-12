using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using PortnoxMCP.Tools;
using PortnoxMCP;

public class GetPortnoxSiteTests
{
    private readonly Mock<IPortnoxApiClient> _mockClient = new Mock<IPortnoxApiClient>();
    private readonly Mock<ILogger<GetPortnoxSite>> _mockLogger = new Mock<ILogger<GetPortnoxSite>>();
    private readonly GetPortnoxSite _service;

    public GetPortnoxSiteTests()
    {
        _service = new GetPortnoxSite(_mockClient.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task GetSitesAsync_ReturnsSites_OnSuccess()
    {
        var json = "[{\"Name\":\"SiteA\"}]";
        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(json)
        };
    _mockClient.Setup(c => c.SendAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<bool>())).ReturnsAsync(response);

        var result = await _service.GetSitesAsync();
        Assert.Single(result);
    }

    [Fact]
    public async Task GetSitesAsync_FiltersByName()
    {
        var json = "[{\"Name\":\"Alpha\"},{\"Name\":\"Beta\"}]";
        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(json)
        };
    _mockClient.Setup(c => c.SendAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<bool>())).ReturnsAsync(response);

        var result = await _service.GetSitesAsync("Alpha");
        Assert.Single(result);
        Assert.Equal("Alpha", result[0].Name);
    }

    [Fact]
    public async Task GetSitesAsync_HandlesObjectWrappedSites()
    {
        var json = "{\"sites\":[{\"Name\":\"SiteA\"}]}";
        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(json)
        };
    _mockClient.Setup(c => c.SendAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<bool>())).ReturnsAsync(response);

        var result = await _service.GetSitesAsync();
        Assert.Single(result);
    }

    [Fact]
    public async Task GetSitesAsync_HandlesEmptyResponse()
    {
        var json = "[]";
        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(json)
        };
    _mockClient.Setup(c => c.SendAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<bool>())).ReturnsAsync(response);

        var result = await _service.GetSitesAsync();
        Assert.Empty(result);
    }
}
