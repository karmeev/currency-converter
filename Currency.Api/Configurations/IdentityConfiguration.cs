using System.Text;
using Currency.Api.Settings;
using Currency.Infrastructure.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;
using Microsoft.IdentityModel.Tokens;

namespace Currency.Api.Configurations;

public static class IdentityConfiguration
{
    public static void ConfigureIdentity(this IServiceCollection services, JwtSettings settings)
    {
        services.AddDataProtection()
            .PersistKeysToFileSystem(new DirectoryInfo(@"/root/.aspnet/DataProtection-Keys"))
            .UseCryptographicAlgorithms(new AuthenticatedEncryptorConfiguration()
            {
                EncryptionAlgorithm = EncryptionAlgorithm.AES_256_CBC,
                ValidationAlgorithm = ValidationAlgorithm.HMACSHA256
            });

        services.AddAuthorization();

        services.AddAuthentication(options =>
            {
                options.DefaultScheme = "DOMINO";
                options.DefaultChallengeScheme = "DOMINO";
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
            });
    }
}