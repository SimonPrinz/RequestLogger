# Request Logger

This package is a middleware for ASP.NET application to log incoming requests.
It does that by utilizing the [Microsoft.Extensions.Logging](https://github.com/dotnet/runtime/tree/main/src/libraries/Microsoft.Extensions.Logging) library.

Depending on which environment you are using the requests will be logged differently.
You can also specify your own format by setting the `RequestLogger` key in the `appsettings.json`.

## Installation

Require the [nuget from the repository](https://github.com/SimonPrinz/RequestLogger/pkgs/nuget/SimonPrinz.RequestLogger), add the services and the middleware in your startup class.

## Usage

```csharp
using SimonPrinz.RequestLogger;

public class Startup
{
    public void ConfigureServices(IServiceCollection pServices)
    {
        [...]
        pServices.AddRequestLogger();
        [...]
    }
    
    public void Configure(IApplicationBuilder pApp)
    {
        [...]
        pApp.UseRequestLogger();
        [...]
    }
}
```

### Default values

Production: `{RemoteAddress} - {RemoteUser} {DateTime} "{Method} {Url} {HttpVersion}" {Status} {ContentLength} "{Referrer}" "{UserAgent}"`  
Development (or everything else): `{Method} {Url}{QueryString} {Status} {ResponseTime} ms - {ContentLength}`

### Available variables

There are currently 12 variables that can be used. Most of them are subject to change, and will probably change at some point in the future (Watch the release notes for those changes!).

- `ContentLength` - The length of the response
- `DateTime` - The DateTime of the request in the format of ISO 8601
- `HttpVersion` - The used HTTP version
- `Method` - The used HTTP method
- `QueryString` - The query string of the request
- `Referrer` - The referrer of the request
- `RemoteAddress` - The remote address of the user
- `RemoteUser` - The name of the claims identity associated with this claims principal
- `ResponseTime` - The response time in milliseconds of the response
- `Status` - The status code of the response
- `Url` - The request path
- `UserAgent` - The user-agent of the request
