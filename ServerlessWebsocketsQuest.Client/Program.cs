using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using BlazorApp.Client;
using IO.Ably;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.Configuration["API_Prefix"] ?? builder.HostEnvironment.BaseAddress) });
var clientOptions = new ClientOptions() { AuthUrl = new Uri("http://localhost:7071/api/CreateTokenRequest") };
builder.Services.AddSingleton<IRealtimeClient>(sp => new AblyRealtime(clientOptions));

await builder.Build().RunAsync();
