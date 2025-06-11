using System.Threading.RateLimiting;
using Asp.Versioning;
using Currency.Api.Exceptions;
using Currency.Api.Settings;

namespace Currency.Api.Configurations;

public static class ApiConfiguration
{
    //TODO: add unit tests here
    public static ApiSettings ConfigureSettings(this ConfigurationManager configurationManager, string env)
    {
        var configurationRoot = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", false, true)
            .AddJsonFile($"appsettings.{env}.json", false, true)
            .AddEnvironmentVariables()
            .Build();

        configurationManager.AddConfiguration(configurationRoot);
        try
        {
            var settings = configurationManager.Get<ApiSettings>();
            return settings;
        }
        catch (InvalidOperationException)
        {
            return StartupException.ThrowIfConfigurationIncorrect<ApiSettings>();
        }
    }

    public static void ConfigureVersioning(this IServiceCollection services)
    {
        services.AddApiVersioning(options =>
            {
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.ReportApiVersions = true;
                options.ApiVersionReader = ApiVersionReader.Combine(
                    new UrlSegmentApiVersionReader(),
                    new HeaderApiVersionReader("X-Api-Version")
                );
            })
            .AddApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'V";
                options.SubstituteApiVersionInUrl = true;
            });
    }

    public static void ConfigureRateLimiter(this IServiceCollection services, RateLimiterSettings settings)
    {
        services.AddRateLimiter(options =>
        {
            options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
            {
                return RateLimitPartition.GetFixedWindowLimiter(GetClient(httpContext), _ =>
                    new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = settings.PermitLimit,
                        Window = TimeSpan.FromMilliseconds(settings.DurationMilliseconds),
                        QueueProcessingOrder = settings.QueueOrder,
                        QueueLimit = settings.QueueLimit
                    });
            });

            options.RejectionStatusCode = 429;
        });
        return;

        string GetClient(HttpContext context)
        {
            return context.User.Identity?.Name ?? context.Connection.RemoteIpAddress?.ToString() ?? "anonymous";
        }
    }
}