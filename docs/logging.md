# Logging Configuration

PortnoxMCP uses centralized, structured logging to aid in monitoring, debugging, and auditing.

## 🏗️ Configuration
- Logging is configured in `Program.cs` using `Microsoft.Extensions.Logging`.
- Log level and output can be set via environment variables or configuration files (e.g., `ASPNETCORE_ENVIRONMENT`).
- Example (in `Program.cs`):
	```csharp
	builder.Logging.SetMinimumLevel(LogLevel.Information);
	builder.Logging.AddConsole();
	```

## 📝 Log Output
- Logs are output to the console by default; can be extended to files or external systems (e.g., Seq, ELK, Azure Monitor).
- Log entries are structured (JSON or plain text) and include timestamps, log level, and message.
- Example log entry:
	```json
	{
		"Timestamp": "2025-08-10T12:34:56Z",
		"Level": "Information",
		"Message": "User login succeeded.",
		"SourceContext": "PortnoxMCP.Tools.GetPortnoxDevices"
	}
	```

## 🎚️ Log Levels
- Supported levels: Trace, Debug, Information, Warning, Error, Critical.
- Default is `Information` in production, `Debug` or `Trace` in development.
- Adjust log level via environment variable:
	```sh
	ASPNETCORE_ENVIRONMENT=Development
	```

## 🔄 Log Rotation & Retention
- For file or external logging, configure log rotation and retention to avoid disk space issues.
- Use log aggregation tools for long-term storage and analysis.

## 🚫 Sensitive Data
- Sensitive data (API keys, passwords, PII) must never be logged.
- Review logs regularly for accidental exposure.
- See [`SECURITY.md`](../SECURITY.md) for security policies.

## 🛠️ Troubleshooting
- Increase log level to `Debug` or `Trace` when diagnosing issues.
- Include correlation IDs or request IDs in logs for distributed tracing.
- Use log search and filtering tools to quickly find relevant entries.
