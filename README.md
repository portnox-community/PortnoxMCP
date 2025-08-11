

# 🚦 PortnoxMCP

PortnoxMCP is a .NET-based project that bridges the Portnox Clear API with the Model Context Protocol (MCP) ecosystem. It enables secure, scalable, and maintainable integration of Portnox Clear with LLMs and other MCP-compliant tools.

## 📑 Table of Contents

- [✨ Features](#-features)
- [⚡ Quick Start](#-quick-start)
- [🧰 Tools](#-tools)
- [📚 Documentation](#-documentation)
- [🏗️ Architecture & Best Practices](#-architecture--best-practices)
- [🌐 Remote Portnox MCP Server](#-remote-portnox-mcp-server)
- [🔌 Server Port Configuration](#-server-port-configuration)
- [⚙️ Environment Variables](#️-environment-variables)
- [🧑‍💻 Local Development: Build & Run with Docker](#-local-development-build--run-with-docker)
- [🤝 Community & Support](#-community--support)


## ✨ Features
- 🛠️ Exposes Portnox Clear API operations as MCP tools
- 🔒 Secure authentication and robust error handling
- 🐳 Containerized with Docker and Docker Compose
- 🧩 Extensible and maintainable architecture


## ⚡ Quick Start
See [`Checklist.md`](Checklist.md) and [`FOUNDATION.md`](FOUNDATION.md) for setup and requirements.



## 🧰 Tools

The following MCP tools are currently implemented and available:

- **GetPortnoxDevices**
	- Retrieves devices from the Portnox API.
	- Supports filtering by device ID, device name, and advanced query/search parameters.
	- Handles pagination and progress notifications.

- **GetPortnoxSite**
	- Retrieves site information from the Portnox API.
	- Supports filtering by site name or ID.

- **GetPortnoxMACAccounts**
	- Retrieves MAC-based accounts from the Portnox API.
	- Supports filtering by account name.

> For full details on tool parameters and usage, see [`docs/usage_examples.md`](docs/usage_examples.md) and the API documentation.

## 📚 Documentation
- [`FOUNDATION.md`](FOUNDATION.md) — Project foundation, architecture, and rationale
- [`Checklist.md`](Checklist.md) — Setup, tasks, and project status
- [`docs/`](docs/) — API, architecture, CI/CD, logging, monitoring, and more



## 🏗️ Architecture & Best Practices

PortnoxMCP is designed with modern architectural principles and best practices for MCP-compliant servers:


- 🤖 **MCP Protocol Compliance:**
	- Exposes Portnox Clear API operations as MCP tools using the ModelContextProtocol C# SDK.
	- Supports dynamic tool discovery and modular toolsets for extensibility.

- 🐳 **Containerization & Deployment:**
	- Fully containerized with Docker and Docker Compose, supporting local and cloud deployments.
	- Environment-based configuration for secrets, tokens, and toolset selection.

- 🔐 **Security & Authentication:**
	- All API requests use secure bearer token authentication, sourced from environment variables (e.g., `PORTNOXAPIKEY`).
	- TLS 1.2+ enforced for all outbound HTTP requests.

- 📈 **Logging & Observability:**
	- Centralized, structured logging with configurable verbosity and robust error handling.

- 🧩 **Extensibility:**
	- New Portnox API endpoints and MCP tools can be added easily by following the established folder and class structure.

- 🏆 **Best Practices:**
	- Follows open source best practices: clear documentation, code of conduct, security policy, CI/CD, and secrets management.
	- All sensitive configuration is managed via environment variables or secure stores—never hardcoded.


# 🌐 Remote Portnox MCP Server

The Portnox MCP Server can be run remotely and accessed by any compatible MCP host (such as VS Code, Claude, Cursor, and others) that supports HTTP-based MCP servers.


### 📝 Prerequisites

1. 💻 A compatible MCP host with remote server support (e.g., VS Code 1.101+, Claude Desktop, Cursor, etc.)
2. 🚀 The Portnox MCP Server running and accessible over HTTP(S)
3. 🔑 A valid Portnox API key (set as the `PORTNOXAPIKEY` environment variable)


### ⚙️ Configuration Example

To connect your MCP host to a remote Portnox MCP Server, add a configuration block similar to the following (adjust the URL as needed):

---


## 🔌 Server Port Configuration

The server port is configurable via the `MCP_HTTP_PORT` environment variable. By default, the server listens on port 8080. You can override this by setting the variable:

```powershell
$env:MCP_HTTP_PORT=5000
dotnet run --project src/
```


Or in a `.env` file:

```
MCP_HTTP_PORT=5000
```

This is implemented in code as:

```csharp
var portStr = builder.Configuration["MCP_HTTP_PORT"] ?? Environment.GetEnvironmentVariable("MCP_HTTP_PORT");
```


If the port is already in use, set `MCP_HTTP_PORT` to a free port and update your client configuration accordingly.

```json
{
	"servers": {
		"portnox": {
			"type": "http",
			"url": "https://your-portnox-mcp-server.example.com/mcp/",
			"headers": {
				"Authorization": "Bearer ${input:portnox_api_key}"
			}
		}
	},
	"inputs": [
		{
			"type": "promptString",
			"id": "portnox_api_key",
			"description": "Portnox API Key",
			"password": true
		}
	]
}
```


> **ℹ️ Note:** The actual configuration syntax may vary depending on your MCP host. Refer to your host's documentation for details.


### 🔒 Security

- ✅ Always use HTTPS to connect to the remote Portnox MCP Server.
- 🚫 Never hardcode API keys; use environment variables or secure input prompts.
- 🔐 The server enforces TLS 1.2+ and bearer token authentication for all API requests.


### 🚀 Usage

Once configured, you can use all available Portnox MCP tools from your MCP host, just as you would with any other MCP-compliant server.


## ⚙️ Environment Variables

The following environment variables can be set to configure PortnoxMCP:

| Variable                        | Description                                                      | Default |
|----------------------------------|------------------------------------------------------------------|---------|
| PORTNOXAPIKEY                    | Portnox API key (required)                                       |         |
| ASPNETCORE_ENVIRONMENT           | .NET environment (Development/Production)                        | Development |
| PORTNOX_MAX_RETRIES              | Max retries for 429 (Too Many Requests) responses                | 3       |
| PORTNOX_INITIAL_DELAY_SECONDS    | Initial delay (in seconds) before retrying 429 responses         | 1       |
| MCP_HTTP_IDLE_TIMEOUT_SECONDS    | Idle timeout (in seconds) for HTTPTransport (MCP server). Use -1 for infinite. | -1 (infinite) |


Set these in your `.env` file or as environment variables in your deployment.



## 🧑‍💻 Local Development: Build & Run with Docker

PortnoxMCP is fully containerized for easy local development and deployment. You can use either Docker Compose or standard Docker commands.


### 📝 Prerequisites

- 🐳 [Docker Desktop](https://www.docker.com/products/docker-desktop/) installed and running
- 🧩 (Optional) [Docker Compose](https://docs.docker.com/compose/) for multi-service orchestration


### 🐳 Using Docker Compose (Recommended)

1. 📥 **Clone the repository:**
	```sh
	git clone <repo-url>
	cd PortnoxMCP
	```

2. 📝 **Set up your environment variables:**
	```sh
	cp .env.example .env
	# Edit .env to add your PORTNOXAPIKEY and other values
	```

3. 🏗️ **Build and run the stack:**
	```sh
	docker-compose up --build
	```
	This will build the application image and start the PortnoxMCP server on port 8080 by default.

4. ⚙️ **Custom Configuration:**
	- You can override environment variables in your `.env` file or pass them at runtime:
	```sh
	docker-compose run -e PORTNOXAPIKEY=your-key-here portnoxmcp
	```

5. 🛑 **Stopping the server:**
	```sh
	docker-compose down
	```


### 🐋 Using Docker Build & Run (Standalone)

1. 🏗️ **Build the Docker image:**
	```sh
	docker build -t portnoxmcp .
	```

2. 🚀 **Run the container:**
	```sh
	docker run -p 8080:8080 --env-file .env portnoxmcp
	```
	This will start the server on port 8080 using environment variables from your `.env` file.


---

For more details, see [`FOUNDATION.md`](FOUNDATION.md) and [`docker-compose.yml`](docker-compose.yml).

---

## 🤝 Community & Support

- Please see [`CODE_OF_CONDUCT.md`](CODE_OF_CONDUCT.md) and [`CONTRIBUTING.md`](CONTRIBUTING.md) for community guidelines and how to get involved.
- For questions, open an issue or contact the maintainers.
