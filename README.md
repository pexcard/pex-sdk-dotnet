# PEX Card API SDK for .NET

.NET SDK to communicate with [PEX Card API](https://developer.pexcard.com/docs4). To apply for API credentials please follow Sign Up instructions on the [PEX developer portal](https://developer.pexcard.com).

## Using PexCard.Api.Client with ASP.NET Core HttpClient factory and Polly

###### Depended nuget packages:

[Microsoft.Extensions.DependencyInjection](https://www.nuget.org/packages/Microsoft.Extensions.DependencyInjection)
[Microsoft.Extensions.Http](https://www.nuget.org/packages/Microsoft.Extensions.Http)
[Microsoft.Extensions.Http.Polly](https://www.nuget.org/packages/Microsoft.Extensions.Http.Polly)
[Polly](https://www.nuget.org/packages/Polly)
[Polly.Extensions.Http](https://www.nuget.org/packages/Polly.Extensions.Http)

```csharp
/**********************************************************/
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
/**********************************************************/
```

Once PexApiClient is registered in service collection it could be injected as IPexApiClient.

## License

[MIT](LICENSE)
