


<p align="center">
	<img src="https://registry.npmmirror.com/@lobehub/icons-static-png/latest/files/dark/mcp.png" alt="MCP Logo" width="120" />
</p>

<p align="center">
	<img src="https://img.shields.io/badge/.NET-8.0-blueviolet?logo=dotnet" alt=".NET 8.0" height="20"/>
	<img src="https://img.shields.io/badge/Docker-ready-blue?logo=docker" alt="Docker" height="20"/>
</p>

<p align="center">
	<a href="https://github.com/CalebBartleMAA/PortnoxMCP/actions/workflows/ci.yml"><img src="https://img.shields.io/github/actions/workflow/status/CalebBartleMAA/PortnoxMCP/ci.yml?branch=main&label=CI" alt="Build Status"></a>
	<a href="https://github.com/CalebBartleMAA/PortnoxMCP/blob/main/LICENSE"><img src="https://img.shields.io/github/license/CalebBartleMAA/PortnoxMCP.svg" alt="License"></a>
	<a href="https://github.com/CalebBartleMAA/PortnoxMCP/releases"><img src="https://img.shields.io/github/v/release/CalebBartleMAA/PortnoxMCP?include_prereleases&label=release" alt="Release"></a>
	<a href="https://github.com/CalebBartleMAA/PortnoxMCP/issues"><img src="https://img.shields.io/github/issues/CalebBartleMAA/PortnoxMCP.svg" alt="Issues"></a>
</p>
<p align="center">
	<a href="CONTRIBUTING.md"><img src="https://img.shields.io/badge/contributions-welcome-brightgreen.svg?style=flat" alt="Contributions welcome"></a>
	<a href="SECURITY.md"><img src="https://img.shields.io/badge/security-policy-important.svg?color=orange" alt="Security Policy"></a>
	<img src="https://img.shields.io/badge/status-beta-blueviolet" alt="Project Status: Beta" />
</p>
</p>

<p align="center">
	<b>Open source Portnox MCP Server for LLMs with Portnox CLEAR</b>
</p>

<p align="center">
	<a href="#features">Features</a> •
	<a href="#quick-start">Quick Start</a> •
	<a href="#tools">Tools</a> •
	<a href="#documentation">Docs</a> •
	<a href="#architecture--best-practices">Architecture</a> •
	<a href="#remote-portnox-mcp-server">Remote Server</a> •
	<a href="#environment-variables">Env Vars</a> •
	<a href="#local-development-build--run-with-docker">Docker</a> •
	<a href="#community--support">Community</a> •
	<a href="https://github.com/CalebBartleMAA/PortnoxMCP/issues">Issues</a> •
	<a href="https://portnox.com/">Portnox.com</a>
</p>

# Portnox MCP Server

Portnox MCP Server is an open source, community driven .NET-based project that bridges the Portnox Clear API with the Model Context Protocol (MCP) ecosystem. It enables secure, scalable, and maintainable integration of Portnox Clear with LLMs and other MCP-compliant tools.

> **Disclaimer:** This project is not affiliated with, endorsed by, or supported by Portnox™. For official Portnox products, support, and documentation, please visit [portnox.com](https://portnox.com/) and [docs.portnox.com](https://docs.portnox.com/).

## 📑 Table of Contents

- [✨ Features](#-features)
- [⚡ Quick Start](#-quick-start)
- [🧰 Tools](#-tools)
- [📚 Documentation](#-documentation)
- [🏗️ Architecture & Best Practices](#-architecture--best-practices)
- [🌐 Remote Portnox MCP Server](#-remote-portnox-mcp-server)
- [⚙️ Environment Variables](#️-environment-variables)
- [🧑‍💻 Local Development: Build & Run with Docker](#-local-development-build--run-with-docker)
- [🤝 Community & Support](#-community--support)

## ✨ Features
- 🛠️ Exposes Portnox Clear API operations as MCP tools
- 🔒 Secure authentication and robust error handling
- 🐳 Containerized with Docker and Docker Compose
- 🧩 Extensible and maintainable architecture


## 📝 TODO / Upcoming Features
- **Client Side or Server Side Auth Support**
- **Audit logging:** Track all API and tool invocations for compliance and troubleshooting.
- **Advanced filtering and search:**
	- More flexible query options for devices, sites, and accounts.
	- Document and improve device list filter options:
		- **Default Enabled:** `IntuneDeviceData`
		- **Default Disabled:** `MdmEnrollmentInfo`, `ExtendedOSInfo`, `MobileGsmSettings`, `MobilePhoneNumber`, `MobileWifiSettings`, `OrganizationPresence`, `OsVersion`, `ProfilingData`, `Status`
	- Intent-based calls: Look up device owner via `$Data.IntuneDeviceData.DeviceOwners`
	- Single device lookup: Filter out `Credentials` completely
	- Known broken filters (Portnox side): `AccountNameUI`
- **Bulk operations:** Support for batch device/account updates and actions.
- **Enhanced error reporting:** More detailed error messages and troubleshooting guidance.

_Have a feature request? Open an issue or contribute!_


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

## Visual Studio Code MCP Configuration

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


> **ℹ️ Note:** The actual configuration syntax may vary depending on your MCP host. Refer to your IDE documentation for details.


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
| MCP_HTTP_PORT                    | Port for the MCP HTTP server.                                    | 8080    |
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
