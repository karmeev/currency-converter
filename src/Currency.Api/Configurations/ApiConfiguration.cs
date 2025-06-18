using System.Text;
using System.Threading.RateLimiting;
using Asp.Versioning;
using Currency.Api.Models;
using Currency.Api.Schemes;
using Currency.Api.Settings;
using Currency.Data.Settings;
using Currency.Infrastructure.Settings;
using Currency.Services.Application.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Serilog.Events;

namespace Currency.Api.Configurations;

public static class ApiConfiguration
{
    public static void AddSettings(this IServiceCollection services, string env, out StartupSettings settings)
    {
        var appVersion = Environment.GetEnvironmentVariable("APP_VERSION") ?? "1.0.0";

        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{env}.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();

        services.AddOptions();

        // Bind options
        services.Configure<RateLimiterSettings>(configuration.GetSection("RateLimiter"));
        services.Configure<WorkerSettings>(configuration.GetSection("Services:Workers"));
        services.Configure<JwtSettings>(configuration.GetSection("Infrastructure:Jwt"));
        services.Configure<RedisSettings>(configuration.GetSection("Infrastructure:Redis"));
        services.Configure<FrankfurterSettings>(configuration.GetSection("Infrastructure:Integrations:Frankfurter"));
        services.Configure<CacheSettings>(configuration.GetSection("Data:Cache"));

        // Logger settings
        var loggerSection = configuration.GetSection("Infrastructure:Logger");
        var loggerSettings = new LoggerSettings
        {
            ConsoleTemplate = loggerSection.GetValue<string>("Console:ConsoleTemplate"),
            ElasticIndexFormat = loggerSection.GetValue<string>("ELK:IndexFormat"),
            ElasticEndpoint = loggerSection.GetValue<string>("ELK:LogEndpoint"),
            JaegerEndpoint = loggerSection.GetValue<string>("Telemetry:JaegerEndpoint"),
            ConsoleLogLevel = Enum.TryParse(loggerSection.GetValue<string>("Console:LogLevel"),
                out LogEventLevel consoleLevel)
                ? consoleLevel
                : LogEventLevel.Information,
            ElasticLogLevel = Enum.TryParse(loggerSection.GetValue<string>("ELK:LogLevel"), out LogEventLevel elkLevel)
                ? elkLevel
                : LogEventLevel.Information,
            AppVersion = appVersion,
            Application = configuration.GetValue<string>("Application"),
            Environment = env,
            EnableDebugOptions = loggerSection.GetValue<bool>("EnableDebugOptions"),
            DisableLogger = loggerSection.GetValue<bool>("DisableLogger")
        };

        settings = new StartupSettings
        {
            DataProtectionKeysDirectory = configuration.GetValue<string>("DataProtectionKeysDirectory"),
            RateLimiter = configuration.GetSection("RateLimiter").Get<RateLimiterSettings>(),
            Jwt = configuration.GetSection("Infrastructure:Jwt").Get<JwtSettings>(),
            Integrations = new IntegrationsSettings
            {
                Frankfurter = configuration.GetSection("Infrastructure:Integrations:Frankfurter")
                    .Get<FrankfurterSettings>()
            },
            LoggerSettings = loggerSettings,
            ServicesSettings = new ServicesSettings(configuration.GetSection("Services:Workers")
                .Get<WorkerSettings>())
        };
    }

    public static void AddVersioning(this IServiceCollection services)
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

    public static void AddRateLimiter(this IServiceCollection services, StartupSettings settings)
    {
        var config = settings.RateLimiter;
        services.AddRateLimiter(options =>
        {
            options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
            {
                return RateLimitPartition.GetFixedWindowLimiter(GetUser(httpContext), _ =>
                    new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = config.PermitLimit,
                        Window = TimeSpan.FromMilliseconds(config.DurationMilliseconds),
                        QueueProcessingOrder = config.QueueOrder,
                        QueueLimit = config.QueueLimit
                    });
            });

            options.RejectionStatusCode = config.RejectionStatusCode;
        });
        return;

        string GetUser(HttpContext context)
        {
            return context.User.Identity?.Name ?? context.Connection.RemoteIpAddress?.ToString() ?? "anonymous";
        }
    }

    public static void AddIdentity(this IServiceCollection services, StartupSettings startupSettings)
    {
        var settings = startupSettings.Jwt;
        services.AddDataProtection()
            .PersistKeysToFileSystem(new DirectoryInfo(startupSettings.DataProtectionKeysDirectory))
            .UseCryptographicAlgorithms(new AuthenticatedEncryptorConfiguration
            {
                EncryptionAlgorithm = EncryptionAlgorithm.AES_256_CBC,
                ValidationAlgorithm = ValidationAlgorithm.HMACSHA256
            });

        services.AddAuthorization();

        services.AddAuthentication(options => { options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme; })
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = settings.Issuer,
                    ValidateAudience = true,
                    ValidAudience = settings.Audience,
                    ValidateLifetime = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.SecurityKey)),
                    ValidateIssuerSigningKey = true,
                    ClockSkew = TimeSpan.Zero
                };

                options.Events = new JwtBearerEvents
                {
                    OnChallenge = async context =>
                    {
                        context.HandleResponse();
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        context.Response.ContentType = "application/json";

                        var response = new ErrorResponseScheme
                        {
                            Error = ErrorMessage.Unauthorized,
                            Message = "Authentication is required to access this resource."
                        };

                        await context.Response.WriteAsync(JsonConvert.SerializeObject(response));
                    },
                    OnForbidden = async context =>
                    {
                        context.Response.StatusCode = StatusCodes.Status403Forbidden;
                        context.Response.ContentType = "application/json";

                        var response = new ErrorResponseScheme
                        {
                            Error = ErrorMessage.Forbidden,
                            Message = "You do not have sufficient permissions to access this resource."
                        };

                        await context.Response.WriteAsync(JsonConvert.SerializeObject(response));
                    }
                };
            });
    }

    public static void AddCustomBehavior(this IServiceCollection services)
    {
        services.Configure<ApiBehaviorOptions>(options =>
        {
            options.InvalidModelStateResponseFactory = context =>
            {
                var errors = context.ModelState
                    .Where(e => e.Value?.Errors.Count > 0)
                    .ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value!.Errors.Select(e => e.ErrorMessage).ToArray()
                    );

                var response = new ErrorResponseScheme
                {
                    Error = ErrorMessage.ValidationError,
                    Message = "One or more validation errors occurred.",
                    Details = errors
                };

                return new BadRequestObjectResult(response)
                {
                    ContentTypes = { "application/json" }
                };
            };
        });
    }
}