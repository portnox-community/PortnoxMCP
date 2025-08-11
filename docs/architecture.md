# Architecture Overview

PortnoxMCP bridges the Portnox Clear API with the Model Context Protocol (MCP) using .NET and the ModelContextProtocol SDK. This enables secure, modular, and scalable integration with LLMs and other MCP-compliant tools.

## 🗺️ High-Level Architecture Diagram

```
┌────────────┐      ┌────────────────────┐      ┌────────────────────┐
│  MCP Host │◀───▶│ PortnoxMCP Server  │◀───▶│ Portnox Clear API  │
└────────────┘      └────────────────────┘      └────────────────────┘
			▲                    ▲
			│                    │
			▼                    ▼
	LLMs, IDEs,         ModelContextProtocol
	or other clients    C# SDK, .NET
```

## 🧩 Major Components

- **MCP Server Layer:**
	- Exposes Portnox Clear API operations as MCP tools.
	- Handles authentication, request routing, and response formatting.
- **Portnox API Client:**
	- Encapsulates all HTTP communication with the Portnox Clear API.
	- Manages authentication, retries, and error handling.
- **Tool Classes:**
	- Each Portnox API operation is wrapped as a tool class (see `Tools/`).
	- Tools are registered and discovered dynamically.
- **Configuration & Secrets:**
	- All sensitive data is managed via environment variables or secure stores.
- **Logging & Monitoring:**
	- Centralized, structured logging and health checks for observability.

## 🔄 Data Flow

1. MCP client (e.g., LLM, IDE) sends a request to the PortnoxMCP server.
2. The server authenticates the request and routes it to the appropriate tool.
3. The tool invokes the Portnox API client, which communicates with the Portnox Clear API.
4. The response is returned to the client in a standardized MCP format.

## 🧱 Extensibility

- Add new Portnox API endpoints by creating new tool classes in the `Tools/` directory.
- Register new tools in the server startup configuration.
- Follow the established folder and class structure for consistency.

## 📚 Related Documentation
- See [`FOUNDATION.md`](../FOUNDATION.md) for requirements and rationale.
- See [`Checklist.md`](../Checklist.md) for project progress.
- See [`api_overview.md`](api_overview.md) for endpoint and usage details.
