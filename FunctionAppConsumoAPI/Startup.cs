using System;
using System.Net.Http;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using FunctionAppConsumoAPI.Clients;

[assembly: FunctionsStartup(typeof(FunctionAppConsumoAPI.Startup))]
namespace FunctionAppConsumoAPI
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var retryPolicy = Policy
                .HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
                .RetryAsync(2, onRetry: (message, retryCount) =>
                {
                    var backColor = Console.BackgroundColor; 
                    Console.BackgroundColor = ConsoleColor.Yellow;
                    
                    var foreColor = Console.ForegroundColor;
                    Console.ForegroundColor = ConsoleColor.Black;
                    
                    Console.Out.WriteLine($"RequestMessage: {message.Result.RequestMessage}");
                    Console.Out.WriteLine($"Content: {message.Result.Content.ReadAsStringAsync().Result}");
                    Console.Out.WriteLine($"ReasonPhrase: {message.Result.ReasonPhrase}");
                    Console.Out.WriteLine($"Retentativa: {retryCount}");

                    Console.BackgroundColor = backColor;
                    Console.ForegroundColor = foreColor;
                });

            builder.Services.AddHttpClient<APIContagemClient>()
                .AddPolicyHandler(retryPolicy);
        }
    }
}