# API Overview

PortnoxMCP exposes Portnox Clear API operations as Model Context Protocol (MCP) tools using the ModelContextProtocol C# SDK. This enables secure, modular, and scalable integration with LLMs and other MCP-compliant clients.

## 🔐 Authentication
- All endpoints require authentication via a bearer token, provided as the `PORTNOXAPIKEY` environment variable (see `.env.example`).
- Never hardcode API keys; always use environment variables or secure secrets management.
- The bearer token must be sent in the `Authorization` header:
	```http
	Authorization: Bearer <your-portnox-api-key>
	```

## 🌐 Endpoint Structure
- MCP tools are exposed as HTTP endpoints under `/mcp/tools/<ToolName>/<MethodName>`.
- Example: `POST /mcp/tools/GetPortnoxDevices/GetDevicesAsync`
- All requests and responses use JSON.

### Example Request
```http
POST /mcp/tools/GetPortnoxDevices/GetDevicesAsync HTTP/1.1
Host: your-portnox-mcp-server.example.com
Authorization: Bearer <your-portnox-api-key>
Content-Type: application/json

{
	"deviceId": "12345",
	"pageNumber": 1,
	"pageSize": 10
}
```

### Example Response
```json
[
	{
		"DeviceId": "12345",
		"DeviceName": "Workstation-01",
		"Status": "Active",
		...
	},
	...
]
```

## 🔒 Security
- All HTTP requests enforce TLS 1.2+ for secure communication.
- Sensitive data is never logged or exposed in error messages.

## ⚠️ Error Handling & Logging
- Robust error handling is implemented throughout the API layer.
- Errors are returned as structured JSON with clear messages and codes.
- Example error response:
	```json
	{
		"error": {
			"code": 401,
			"message": "Unauthorized: Invalid or missing API key."
		}
	}
	```
- Logging is configurable and centralized (see `logging.md`).
## 🚦 Rate Limiting
- The Portnox Clear API and PortnoxMCP may enforce rate limits to prevent abuse.
- If you receive a `429 Too Many Requests` response, respect the `Retry-After` header and consider reducing request frequency.
## 🆘 Support
- For API questions, bug reports, or feature requests, please open an issue on GitHub or contact the maintainers.

## 🧩 Extensibility
- New Portnox API endpoints can be added as MCP tools by following the established folder and class structure.
- See `FOUNDATION.md` for architectural guidelines.

## 🏷️ Versioning
- The API follows [Semantic Versioning](https://semver.org/).
- All changes and releases are documented in `CHANGELOG.md`.

## 📚 Further Reading
- See [`FOUNDATION.md`](../FOUNDATION.md) for requirements and architecture.
- See [`Checklist.md`](../Checklist.md) for project status and setup.
- See [`usage_examples.md`](usage_examples.md) for more example requests and responses.
- See [`secrets_management.md`](secrets_management.md) for secure API key handling.
