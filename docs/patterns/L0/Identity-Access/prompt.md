# Identity & Access Copilot Prompts

Use these prompts with GitHub Copilot to generate identity and access code following the L0 Identity & Access pattern.

## Basic Authentication Setup

```
Apply L0/Identity-Access pattern to add JWT authentication to my ASP.NET Core API. 
Use Entra ID, configure RBAC with Reader/Admin roles, add Managed Identity for outbound calls.
Include authentication middleware, authorization policies, and sample controller.
Generate unit tests and integration tests.
```

## Service-to-Service Authentication

```
Generate L0/Identity-Access service-to-service client using Managed Identity.
Target scope: api://beneficiary-service/.default
Add token caching, retry logic, and proper error handling.
Include HttpClient configuration and dependency injection setup.
```

## Advanced Authorization

```
Implement L0/Identity-Access with custom authorization policies.
Add tenant isolation, permission-based access, and audit logging.
Include claims transformation, custom middleware, and security headers.
Generate tests for different user roles and tenant scenarios.
```

## Local Development Setup

```
Create L0/Identity-Access development configuration.
Use Azure AD app registration for local testing.
Include docker-compose with Azurite and proper CORS setup.
Add development user secrets and environment configuration.
```

## Configuration Examples

### Basic Setup
- `--auth=entra-id`
- `--roles=reader,admin`
- `--managed-identity=true`

### Advanced Setup
- `--tenant-isolation=true`
- `--custom-claims=true`
- `--audit-logging=true`
- `--policy-based=true`

### Development Setup
- `--local-dev=true`
- `--use-emulator=true`
- `--cors-enabled=true`