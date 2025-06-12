using Currency.Infrastructure.Contracts.Integrations;
using Polly;
using Polly.CircuitBreaker;
using Polly.Extensions.Http;
using Polly.Retry;
using Polly.Timeout;

namespace Currency.Api.Configurations;

public static class ThirdPartyConfiguration
{
    public static void ConfigureThirdParty(this IServiceCollection services)
    {
        services.AddHttpClient(IntegrationConst.Frankfurter, client =>
            {
                client.BaseAddress = new Uri("https://api.frankfurter.dev");
            })
            .AddPolicyHandler(GetFrankfurterTimeoutPolicy())
            .AddPolicyHandler(GetFrankfurterRetryPolicy())
            .AddPolicyHandler(GetFrankfurterCircuitBreakerPolicy());
    }
    
    private static AsyncTimeoutPolicy<HttpResponseMessage> GetFrankfurterTimeoutPolicy()
    {
        return Policy.TimeoutAsync<HttpResponseMessage>(TimeSpan.FromSeconds(10));
    }

    
    private static AsyncRetryPolicy<HttpResponseMessage> GetFrankfurterRetryPolicy()
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .WaitAndRetryAsync(
                retryCount: 2,
                sleepDurationProvider: attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)),
                onRetry: (outcome, timespan, retryAttempt, context) =>
                {
                    // TODO: add some logs
                    Console.WriteLine($"{outcome} - {retryAttempt} - {context.PolicyKey}");
                });
    }
    
    private static AsyncCircuitBreakerPolicy<HttpResponseMessage> GetFrankfurterCircuitBreakerPolicy()
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError() // Handles 5xx, 408, network failures
            .CircuitBreakerAsync(
                handledEventsAllowedBeforeBreaking: 4,
                durationOfBreak: TimeSpan.FromSeconds(30),
                onBreak: (outcome, timespan) =>
                {
                    // TODO: add some logs
                    Console.WriteLine($"message");
                },
                onReset: () =>
                {
                    // TODO: add some logs
                    Console.WriteLine($"message");
                },
                onHalfOpen: () =>
                {
                    // TODO: add some logs
                    Console.WriteLine($"message");
                });
    }
}