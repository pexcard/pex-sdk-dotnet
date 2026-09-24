# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with the PEX SDK for .NET codebase.

## Overview

The PEX SDK for .NET provides access to the PEX financial platform via REST API. It enables .NET applications to interact with PEX business expense card services including transactions, cardholders, spending rules, tags, payments, and more.

**Repository**: https://github.com/pexcard/pex-sdk-dotnet

## Build Commands

```bash
# Restore NuGet packages
dotnet restore

# Build all projects
dotnet build

# Build in Release configuration
dotnet build -c Release

# Run all tests
dotnet test

# Run tests with verbose output
dotnet test --logger "console;verbosity=detailed"

# Pack NuGet packages (with version)
dotnet pack src/PexCard.Api.Client.Core/PexCard.Api.Client.Core.csproj -c Release -o ./artifacts
dotnet pack src/PexCard.Api.Client/PexCard.Api.Client.csproj -c Release -o ./artifacts
dotnet pack src/PexCard.Api.AspNetCore/PexCard.Api.AspNetCore.csproj -c Release -o ./artifacts
```

## Project Structure

```
pex-sdk-dotnet/
├── src/
│   ├── PexCard.Api.Client.Core/      # Interfaces, models, enums, extensions
│   ├── PexCard.Api.Client/           # HTTP client implementation
│   └── PexCard.Api.AspNetCore/       # ASP.NET Core DI extensions
├── tests/
│   └── PexCard.Api.Client.Core.Tests/ # xUnit tests
├── build/
│   ├── azure-pipelines.yml           # Release pipeline
│   └── azure-pipelines-debug.yml     # PR validation pipeline
└── PexCard.Api.Client.sln
```

## NuGet Packages

| Package | Description |
|---------|-------------|
| `PexCard.Api.Client.Core` | Interfaces, models, enums, and extension methods |
| `PexCard.Api.Client` | HTTP client implementation with retry policies |
| `PexCard.Api.AspNetCore` | ASP.NET Core integration and DI extensions |

## Architecture

### Framework Targets
- **Source projects**: .NET Standard 2.0 (broad compatibility)
- **Test project**: .NET 8.0

### Key Components

**PexApiClient** (`src/PexCard.Api.Client/PexApiClient.cs`)
- Main HTTP client with 100+ API methods
- Handles authentication, correlation IDs, error handling
- Uses `HttpClient` with configurable retry policies

**PexAuthClient** (`src/PexCard.Api.Client/PexAuthClient.cs`)
- OAuth/authentication client for token management

**Configuration Options** (`src/PexCard.Api.Client/Configure/`)
- `PexApiClientOptions` - Timeout, base URI, logging levels, retry policies
- `PexAuthClientOptions` - Auth endpoint configuration
- `PexRetryPolicyOptions` - Configurable backoff retry for 429, timeouts, 5xx errors

### Dependency Injection

Register clients using extension methods:

```csharp
// Using configure options
services.AddPexApiClient(options =>
{
    options.BaseUri = new Uri("https://coreapi.pexcard.com");
    options.Timeout = TimeSpan.FromMinutes(3);
    options.LogLevelSuccess = LogLevel.Debug;
    options.LogLevelFailure = LogLevel.Warning;
    options.Retries = new PexRetryPolicyOptions
    {
        RetryLogLevel = LogLevel.Warning,
        TooManyRequests = new BackoffRetryPolicy(TimeSpan.FromSeconds(5), 7),
        Timeouts = new BackoffRetryPolicy(TimeSpan.FromSeconds(1), 2),
        ServerErrors = new BackoffRetryPolicy(TimeSpan.FromMilliseconds(100), 1)
    };
});

// Using configuration section
services.AddPexAuthClient(Configuration.GetSection("Pex:Auth"));
services.AddPexApiClient(Configuration.GetSection("Pex:Api"));
```

### Resolver Pattern

Pluggable resolvers for cross-cutting concerns:
- `ICorrelationIdResolver` - Generates/tracks correlation IDs (default: `ext-{Guid}`)
- `IIPAddressResolver` - Resolves client IP for `X-Forwarded-For` header
- `HttpContextIpAddressResolver` (AspNetCore) - Extracts IP from HttpContext

### Exception Handling

```csharp
public class PexApiClientException : Exception
{
    public HttpStatusCode Code { get; }
    public string CorrelationId { get; }
}
```

All API errors include HTTP status code and correlation ID for debugging.

## Code Patterns

### Naming Conventions
- Interfaces: `IPex*Client`, `I*Resolver`
- Models: `*Model`, `*RequestModel`, `*ResponseModel`
- Extensions: Internal, marked with `InternalsVisibleTo` for tests

### API Method Pattern
```csharp
public async Task<ResponseModel> GetSomething(string externalToken, CancellationToken cancelToken = default)
{
    var requestUriBuilder = new UriBuilder(new Uri(BaseUri, "V4/Endpoint"));

    var request = new HttpRequestMessage(HttpMethod.Get, requestUriBuilder.Uri);
    request.SetPexCorrelationIdHeader(_correlationIdResolver.GetValue());
    request.SetPexAcceptJsonHeader();
    request.SetPexAuthorizationTokenHeader(externalToken);

    var response = await _httpClient.SendAsync(request, cancelToken);

    return await HandleHttpResponseMessage<ResponseModel>(response);
}
```

### Extension Methods (`src/PexCard.Api.Client/Extensions/`)
- `SetPexAuthorizationBearerHeader` / `SetPexAuthorizationTokenHeader`
- `SetPexCorrelationIdHeader` / `GetPexCorrelationId`
- `SetXForwardForHeader`
- `SetPexAcceptJsonHeader` / `SetPexJsonContent<T>`
- `IsPexJsonContent`

## Testing

### Framework
- **xUnit** 2.7.0 with **Moq** 4.20.70
- Code coverage via `coverlet.collector`

### Test Structure
```
tests/PexCard.Api.Client.Core.Tests/
├── Extensions/
│   ├── PexClientExtensionsTests.cs
│   ├── IPexApiClientExtensionsTests.cs
│   ├── MatchingExtensionsTests.cs
│   ├── TagExtensionsTests.cs
│   └── TransactionListExtensionTests.cs
└── Serialization/
    └── TagDeserializationTests.cs
```

### Running Tests
```bash
# Run all tests
dotnet test

# Run specific test class
dotnet test --filter "FullyQualifiedName~PexClientExtensionsTests"

# With coverage
dotnet test --collect:"XPlat Code Coverage"
```

## Dependencies

### Core Libraries
- `Newtonsoft.Json` 13.0.3 - JSON serialization
- `JsonSubTypes` 2.0.1 - Polymorphic JSON deserialization
- `Microsoft.Extensions.Logging.Abstractions` 6.0.4

### HTTP Client
- `Microsoft.Extensions.Http` 6.0.0 - HttpClient factory
- `Microsoft.Extensions.Http.Polly` 6.0.27 - Resilience policies
- `Polly.Contrib.WaitAndRetry` 1.1.1 - Exponential backoff

### ASP.NET Core Integration
- `Microsoft.AspNetCore.Http` 2.3.0
- `Microsoft.Extensions.DependencyInjection` 6.0.2

## CI/CD Pipeline

### Release Pipeline (`build/azure-pipelines.yml`)
1. Use .NET SDK 8.0.x
2. Restore NuGet packages
3. Run xUnit tests
4. Pack 3 NuGet packages
5. Mend SCA (dependency vulnerability scan)
6. Mend SAST (static code analysis)
7. Publish artifacts

### PR Validation (`build/azure-pipelines-debug.yml`)
- Triggers on PRs to `master`
- Runs tests and security scans

### Versioning
- Semantic versioning 2.0.0
- Format: `{major}.{minor}.{patch}[-{tag}]`
- Patch version auto-incremented via counter

## Key API Categories

The SDK provides methods for:
- **Authentication**: Token management, JWT exchange
- **Business**: Profile, settings, admins, linked businesses
- **Cards**: Fund/zero cards, status updates, spending rulesets
- **Cardholders**: Profiles, details, transactions, groups
- **Transactions**: Query, tags, notes, attachments, approvals
- **Tags**: Configuration, dropdown tags, tag values
- **Payments**: Payment requests, transfers, bill payments
- **Callbacks**: Webhook subscription management
- **Vendor Cards**: Order management

## Important Notes

- All dates sent to PEX API use Eastern Time (EST) via `ToEst()` extension
- Correlation IDs are automatically generated and included in exceptions
- Retry policies handle 429 (rate limiting), timeouts, and 5xx errors
- Internal extensions use `InternalsVisibleTo` for test access
- Source Link enabled for debugging into source via symbols
