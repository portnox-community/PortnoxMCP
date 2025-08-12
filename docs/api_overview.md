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
PortnoxMCP exposes the following HTTP endpoints for its tools:

- `/mcp/tools/GetPortnoxDevices/GetDevicesAsync` — Retrieve devices from the Portnox API
- `/mcp/tools/GetPortnoxSite/GetSiteAsync` — Retrieve site information from the Portnox API
- `/mcp/tools/GetPortnoxMACAccounts/GetMACAccountsAsync` — Retrieve MAC-based accounts from the Portnox API

All endpoints use the POST method and accept JSON request bodies. Replace `<ToolName>` and `<MethodName>` as shown above for each tool.

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
