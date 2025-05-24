using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using InfoDisplay.Services;
using InfoDisplay;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

// Root Components
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// http client
builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)
});

// signalr
builder.Services.AddSingleton(sp =>
{
    var logger = sp.GetRequiredService<ILogger<SignalRClient>>();
    var hubUrl = $"{builder.HostEnvironment.BaseAddress}flightinfo";
    return new SignalRClient(logger, hubUrl);
});

await builder.Build().RunAsync();