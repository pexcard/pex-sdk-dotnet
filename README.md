# PEX SDK for .NET

- [PEX SDK for .NET](#pex-sdk-for-net)
  - [About](#about)
    - [Included Packages](#included-packages)
    - [Prerequisites](#prerequisites)
    - [Versioning](#versioning)
  - [Installation](#installation)
  - [Configuration](#configuration)
    - [ASP.NET Core](#aspnet-core)
          - [Using configure options](#using-configure-options)
          - [Using configuration section](#using-configuration-section)
  - [Examples](#examples)
  - [Support](#support)
    - [Problems](#problems)
    - [Questions](#questions)
    - [Enhancement Requests](#enhancement-requests)

## About

The PEX SDK for .NET provides access to the PEX platform via the REST API. For more information on the API or to obtain your credentials, please sign up on the [PEX Developer Center](https://developer.pexcard.com).

### Included Packages

| Package                                                                            | Description                                                                                  |
| ---------------------------------------------------------------------------------- | -------------------------------------------------------------------------------------------- |
| [PexCard.Api.Client.Core](https://www.nuget.org/packages/PexCard.Api.Client.Core/) | Contains interfaces, models, and extension methods use to describe and work with the PEX API |
| [PexCard.Api.Client](https://www.nuget.org/packages/PexCard.Api.Client/)           | Contains interface implementations and error handling for working with the PEX API           |

### Prerequisites

* A platform supported by [.NET Standard 2.0](https://github.com/dotnet/standard/blob/master/docs/versions/netstandard2.0.md)
* Valid PEX API credentials obtained from https://developer.pexcard.com

### Versioning

All packages following the [semantic versioning 2.0](https://semver.org/) convention.

## Installation

* dotnet CLI
  * `dotnet add package PexCard.Api.Client`
  * `dotnet add package PexCard.Api.Client.Core`

## Configuration

### ASP.NET Core

1. Reference the following packages:
   * [Microsoft.Extensions.DependencyInjection](https://www.nuget.org/packages/Microsoft.Extensions.DependencyInjection)
   * [Microsoft.Extensions.Http](https://www.nuget.org/packages/Microsoft.Extensions.Http)
   * [Microsoft.Extensions.Http.Polly](https://www.nuget.org/packages/Microsoft.Extensions.Http.Polly)
   * [Polly](https://www.nuget.org/packages/Polly)
   * [Polly.Extensions.Http](https://www.nuget.org/packages/Polly.Extensions.Http)
2. Configure the IoC container:

###### Using configure options
   ```csharp
   public void ConfigureServices(IServiceCollection services)
   {
       services.AddPexAuthClient(options =>
       {
           options.BaseUri = new Uri("https://oauth.pexcard.com");
           options.Timeout = TimeSpan.FromSeconds(30);
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
   }
   ```

###### Using configuration section
   ``` json
{
    "Pex": {
        "Auth": {
            "BaseUri": "https://oauth.pexcard.com",
            "Timeout": "00:00:30",
            "LogLevelSuccess": "Debug",
            "LogLevelFailure": "Warning",
            "Retries": {
                "RetryLogLevel": "Warning",
                "TooManyRequests": {
                    "Delay": "00:00:05",
                    "Retries": 7
                },
                "Timeouts": {
                    "Delay": "00:00:01",
                    "Retries": 2
                },
                "ServerErrors": {
                    "Delay": "00:00:00.100",
                    "Retries": 1
                }
            }
        },
        "Api": {
            "BaseUri": "https://coreapi.pexcard.com",
            "Timeout": "00:03:00",
            "LogLevelSuccess": "Debug",
            "LogLevelFailure": "Warning",
            "Retries": {
                "RetryLogLevel": "Warning",
                "TooManyRequests": {
                    "Delay": "00:00:05",
                    "Retries": 7
                },
                "Timeouts": {
                    "Delay": "00:00:01",
                    "Retries": 2
                },
                "ServerErrors": {
                    "Delay": "00:00:00.100",
                    "Retries": 1
                }
            }
        }
    }
}
   ```


   ```csharp
   public void ConfigureServices(IServiceCollection services)
   {
        services.AddPexAuthClient(Configuration.GetSection("Pex:Auth"));
        services.AddPexApiClient(Configuration.GetSection("Pex:Api"));
   }
   ```
3. Inject an instance of `IPexAuthClient` or `IPexApiClient` into the class in which you want to use it.

## Examples

## Support

### Problems

Please create an issue tagged with `bug` including the following information:

- Repro Steps
- Expected Result
- Actual Result
- Package Version
- Operating System

### Questions

Please create an issue tagged with `question`.

### Enhancement Requests

Please create an issue tagged with `enhancement`.
