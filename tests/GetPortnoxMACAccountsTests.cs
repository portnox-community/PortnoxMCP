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

public class GetPortnoxMACAccountsTests
{
    private readonly Mock<IPortnoxApiClient> _mockClient = new Mock<IPortnoxApiClient>();
    private readonly Mock<ILogger<GetPortnoxMACAccounts>> _mockLogger = new Mock<ILogger<GetPortnoxMACAccounts>>();
    private readonly GetPortnoxMACAccounts _service;

    public GetPortnoxMACAccountsTests()
    {
        _service = new GetPortnoxMACAccounts(_mockClient.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task GetMACAccountsAsync_ReturnsAccounts_OnSuccess()
    {
        var json = "{\"MabAccounts\":[{\"AccountName\":\"Test\"}]}";
        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(json)
        };
    _mockClient.Setup(c => c.SendAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<bool>())).ReturnsAsync(response);

        var result = await _service.GetMACAccountsAsync();
        Assert.Single(result);
    }

    [Fact]
    public async Task GetMACAccountsAsync_FiltersByName()
    {
        var json = "{\"MabAccounts\":[{\"AccountName\":\"Alpha\"},{\"AccountName\":\"Beta\"}]}";
        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(json)
        };
    _mockClient.Setup(c => c.SendAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<bool>())).ReturnsAsync(response);

        var result = await _service.GetMACAccountsAsync("Alpha");
        Assert.Single(result);
    }

    [Fact]
    public async Task GetMACAccountsAsync_HandlesJsonParseError()
    {
        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("not json")
        };
    _mockClient.Setup(c => c.SendAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<bool>())).ReturnsAsync(response);

        var result = await _service.GetMACAccountsAsync();
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetMACAccountsAsync_HandlesNoAccounts()
    {
        var json = "{\"MabAccounts\":[]}";
        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(json)
        };
    _mockClient.Setup(c => c.SendAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<bool>())).ReturnsAsync(response);

        var result = await _service.GetMACAccountsAsync();
        Assert.Empty(result);
    }
}
