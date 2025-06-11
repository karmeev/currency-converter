using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Currency.Domain.Login;
using Currency.Domain.Users;
using Currency.Infrastructure.Contracts.JwtBearer;
using Currency.Infrastructure.Settings;
using Microsoft.IdentityModel.Tokens;

namespace Currency.Infrastructure.JwtBearer;

internal class JwtTokenGenerator(JwtSettings jwtSettings) : IJwtTokenGenerator
{
    public IEnumerable<Claim> BuildClaims(string identifier, string username, UserRole role)
    {
        return new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, identifier),
            new(ClaimTypes.Name, username),
            new(ClaimTypes.Role, role.ToString()),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };
    }

    public AccessToken CreateAccessToken(IEnumerable<Claim> claims)
    {
        var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecurityKey));

        var jwtToken = new JwtSecurityToken(
            jwtSettings.Issuer,
            jwtSettings.Audience,
            expires: DateTime.UtcNow.AddMinutes(jwtSettings.AccessTokenTtlInMinutes),
            claims: claims,
            signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256));

        return new AccessToken
        {
            Token = new JwtSecurityTokenHandler().WriteToken(jwtToken),
            ExpiresAt = DateTime.UtcNow.AddMinutes(jwtSettings.AccessTokenTtlInMinutes)
        };
    }

    public string CreateRefreshToken(string username)
    {
        var randomNumber = new byte[64];
        using var random = RandomNumberGenerator.Create();
        random.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }
}