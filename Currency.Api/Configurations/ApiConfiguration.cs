using System.Text;
using System.Threading.RateLimiting;
using Asp.Versioning;
using Currency.Api.Schemes;
using Currency.Api.Settings;
using Currency.Infrastructure.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace Currency.Api.Configurations;

public static class ApiConfiguration
{
    public static void ConfigureSettings(this IServiceCollection services, string env, out StartupSettings settings)
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", false, true)
            .AddJsonFile($"appsettings.{env}.json", true, true)
            .AddEnvironmentVariables()
            .Build();
        
        services.AddOptions();
        services.Configure<RateLimiterSettings>(configuration.GetSection("RateLimiter"));
        services.Configure<JwtSettings>(configuration.GetSection("Infrastructure:Jwt"));
        services.Configure<RedisSettings>(configuration.GetSection("Infrastructure:Redis"));
        services.Configure<FrankfurterSettings>(configuration.GetSection("Infrastructure:Integrations:Frankfurter"));

        settings = new StartupSettings
        {
            DataProtectionKeysDirectory = configuration.GetSection("DataProtectionKeysDirectory").Value,
            RateLimiter = configuration.GetSection("RateLimiter").Get<RateLimiterSettings>(),
            Jwt = configuration.GetSection("Infrastructure:Jwt").Get<JwtSettings>(),
            Integrations = new IntegrationsSettings
            {
                Frankfurter = configuration.GetSection("Infrastructure:Integrations:Frankfurter")
                    .Get<FrankfurterSettings>()
            }
        };
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

    public static void ConfigureRateLimiter(this IServiceCollection services, StartupSettings settings)
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
    
    public static void ConfigureIdentity(this IServiceCollection services, StartupSettings startupSettings)
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

        services.AddAuthentication(options =>
            {
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
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
                            Error = "unauthorized",
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
                            Error = "forbidden",
                            Message = "You do not have sufficient permissions to access this resource."
                        };

                        await context.Response.WriteAsync(JsonConvert.SerializeObject(response));
                    }
                };
            });
    }
}