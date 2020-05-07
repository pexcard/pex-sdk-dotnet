# PEX SDK for .NET

- [PEX SDK for .NET](#pex-sdk-for-net)
  * [About](#about)
    + [Included Packages](#included-packages)
    + [Prerequisites](#prerequisites)
    + [Versioning](#versioning)
  * [Installation](#installation)
      + [dotnet CLI](#dotnet-cli)
  * [Configuration](#configuration)
    + [ASP.NET Core](#aspnet-core)
  * [Examples](#examples)
  * [Support](#support)
    + [Problems](#problems)
    + [Questions](#questions)
    + [Enhancement Requests](#enhancement-requests)

## About
The PEX SDK for .NET provides access to the PEX platform via the REST API. For more information on the API or to obtain your credentials, please sign up on the [PEX Developer Center](https://developer.pexcard.com).

### Included Packages
| Package | Description |
| ----------- | ----------- |
| PexCard.Api.Client.Core | Contains interfaces, models, and extension methods use to describe and work with the PEX API |
| PexCard.Api.Client | Contains interface implementations and error handling for working with the PEX API |

### Prerequisites
* A platform supported by [.NET Standard 2.0](https://github.com/dotnet/standard/blob/master/docs/versions/netstandard2.0.md)
* Valid PEX API credentials obtained from https://developer.pexcard.com

### Versioning
All packages following the versioning convention `{pex-api-version-number}.{major-version-number}.{build-number}` where:
- `{pex-api-version-number}` is the version of the PEX API for which the package was built.
- `{major-version-number}` is the major version of the package. Changes in this number indicate new features, enhancements, or breaking changes.
- `{build-number}`is the internal build number of the package. Channges in this number indicate bug fixes or non-breaking changes.

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
1. Configure the IoC container:
    ```csharp
    public void ConfigureServices(IServiceCollection services)
    {
        var policy = HttpPolicyExtensions
            .HandleTransientHttpError()
            .RetryAsync(3);
    
        services.AddHttpClient<IPexApiClient, PexApiClient>(client =>
            {
    	    // It's recommended to put the base URL string into config file
                client.BaseAddress = new Uri("https://coreapi.pexcard.com/v4");
            })
            .AddPolicyHandler(policy);
    }
    ```
1. Inject an instance of `IPexApiClient` into the class in which you want to use it.

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
