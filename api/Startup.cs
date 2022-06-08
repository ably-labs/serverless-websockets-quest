using System;
using AblyLabs.ServerlessWebsocketsQuest;
using IO.Ably;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(Startup))]
namespace AblyLabs.ServerlessWebsocketsQuest
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var ablyApiKey = Environment.GetEnvironmentVariable("ABLY_APIKEY");
            var ablyClient = new AblyRealtime(ablyApiKey);
            builder.Services.AddSingleton<IRealtimeClient>(ablyClient);
        }
    }
}