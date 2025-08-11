
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ModelContextProtocol.AspNetCore;
using ModelContextProtocol;
using PortnoxMCP;
using PortnoxMCP.Tools;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();



// Configure HTTPTransport idle timeout from environment variable (seconds)
var idleTimeoutStr = builder.Configuration["MCP_HTTP_IDLE_TIMEOUT_SECONDS"] ?? Environment.GetEnvironmentVariable("MCP_HTTP_IDLE_TIMEOUT_SECONDS");
TimeSpan idleTimeout = Timeout.InfiniteTimeSpan;
if (!string.IsNullOrWhiteSpace(idleTimeoutStr) && int.TryParse(idleTimeoutStr, out int idleSeconds) && idleSeconds > 0)
{
    idleTimeout = TimeSpan.FromSeconds(idleSeconds);
}

// Register IHttpContextAccessor for tools/services that need HTTP context
builder.Services.AddHttpContextAccessor();



// Register all tool dependencies for DI
builder.Services.AddTransient<GetPortnoxSite>();
builder.Services.AddTransient<GetPortnoxMACAccounts>();
builder.Services.AddTransient<GetPortnoxDevices>();
builder.Services.AddTransient<Tools>();

builder.Services
    .AddMcpServer()
    .WithHttpTransport(httpOptions =>
    {
        httpOptions.IdleTimeout = idleTimeout;
    })
    .WithTools<Tools>();


// Register PortnoxApiClient for DI so MCP tools can be constructed
builder.Services.AddHttpClient<PortnoxApiClient>(client =>
{
    client.BaseAddress = new Uri("https://clear.portnox.com:8081/CloudPortalBackEnd");
    client.DefaultRequestHeaders.UserAgent.Add(new System.Net.Http.Headers.ProductInfoHeaderValue("portnox-mcp", "1.0"));
});


var app = builder.Build();


// Map both / and /mcp to the MCP protocol handler
app.MapMcp("");    // root /
app.MapMcp("/mcp"); // /mcp

// Get port from environment variable or default to 8080
var portStr = builder.Configuration["MCP_HTTP_PORT"] ?? Environment.GetEnvironmentVariable("MCP_HTTP_PORT");
var port = 8080;
if (!string.IsNullOrWhiteSpace(portStr) && int.TryParse(portStr, out int parsedPort) && parsedPort > 0)
{
    port = parsedPort;
}
app.Run($"http://*:{port}");
