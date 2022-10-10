using System;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using IO.Ably;
using AblyLabs.ServerlessWebsocketsQuest;

[assembly: FunctionsStartup(typeof(Startup))]
namespace AblyLabs.ServerlessWebsocketsQuest
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var ablyApiKey = Environment.GetEnvironmentVariable("ABLY_APIKEY");
            var ablyClient = new AblyRest(ablyApiKey);
            builder.Services.AddSingleton<IRestClient>(ablyClient);
        }
    }
}
