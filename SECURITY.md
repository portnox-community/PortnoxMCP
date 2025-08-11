# Security Policy

## 🔐 Key Principles

- All API keys and secrets must be managed via environment variables or secure secrets management solutions (e.g., Azure Key Vault, AWS Secrets Manager).
- Never hardcode secrets in code, configuration files, or version control.
- Use `.env` files for local development only—never commit them to source control.
- Always use HTTPS for all network communication.
- Regularly update dependencies to address known vulnerabilities.

## 🛡️ Supported Versions

We release security updates for the latest stable version. Please ensure you are running the most recent release for the best protection.

## 📝 Reporting a Vulnerability

If you discover a security vulnerability, please report it responsibly:

1. **Do not** open a public issue.
2. Include as much detail as possible (steps to reproduce, impact, affected versions, etc.)
3. We will acknowledge receipt within 2 business days and work with you to resolve the issue promptly.
4. Once resolved, we will credit you (if desired) in the release notes.

## 🧑‍💻 Security Best Practices for Contributors

- Review code for potential security issues before submitting pull requests.
- Avoid introducing dependencies with known vulnerabilities.
- Validate and sanitize all user input and API responses.
- Follow the principle of least privilege for all access and permissions.

## 📢 Security Updates

Security advisories and updates will be published in the repository and communicated via release notes.

---
