using Currency.Api.Settings;
using Currency.Infrastructure.Contracts.Integrations;
using Currency.Infrastructure.Settings;
using Polly;
using Polly.CircuitBreaker;
using Polly.Extensions.Http;
using Polly.Retry;
using Polly.Timeout;

namespace Currency.Api.Configurations;

public static class ThirdPartyConfiguration
{
    public static void AddThirdParty(this IServiceCollection services, StartupSettings startupSettings)
    {
        services.AddHttpClient();
        
        var frankfurterSettings = startupSettings.Integrations.Frankfurter;
        services.AddHttpClient(IntegrationConst.Frankfurter,
                client => { client.BaseAddress = new Uri(frankfurterSettings.BaseAddress); })
            .AddPolicyHandler(GetFrankfurterTimeoutPolicy(frankfurterSettings))
            .AddPolicyHandler(GetFrankfurterRetryPolicy(frankfurterSettings))
            .AddPolicyHandler(GetFrankfurterCircuitBreakerPolicy(frankfurterSettings));
    }

    private static AsyncTimeoutPolicy<HttpResponseMessage> GetFrankfurterTimeoutPolicy(FrankfurterSettings settings)
    {
        return Policy.TimeoutAsync<HttpResponseMessage>(TimeSpan.FromSeconds(settings.TimeoutSeconds));
    }


    private static AsyncRetryPolicy<HttpResponseMessage> GetFrankfurterRetryPolicy(FrankfurterSettings settings)
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .WaitAndRetryAsync(
                retryCount: settings.RetryCount,
                _ => TimeSpan.FromSeconds(Math.Pow(2, settings.RetryExponentialIntervalSeconds)),
                (outcome, timespan, retryAttempt, context) =>
                {
                    // TODO: add some logs
                    Console.WriteLine($"{outcome} - {retryAttempt} - {context.PolicyKey}");
                });
    }

    private static AsyncCircuitBreakerPolicy<HttpResponseMessage> GetFrankfurterCircuitBreakerPolicy(
        FrankfurterSettings settings)
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .CircuitBreakerAsync(
                handledEventsAllowedBeforeBreaking: settings.CircuitBreakerMaxExceptions,
                durationOfBreak: TimeSpan.FromSeconds(settings.CircuitBreakerDurationBreakSeconds),
                onBreak: (outcome, timespan) =>
                {
                    // TODO: add some logs
                    Console.WriteLine("message");
                },
                onReset: () =>
                {
                    // TODO: add some logs
                    Console.WriteLine("message");
                },
                onHalfOpen: () =>
                {
                    // TODO: add some logs
                    Console.WriteLine("message");
                });
    }
}