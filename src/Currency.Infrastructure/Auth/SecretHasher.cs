using System.Security.Cryptography;
using Currency.Infrastructure.Contracts.Auth;

namespace Currency.Infrastructure.Auth;

internal class SecretHasher : ISecretHasher
{
    private const char SegmentDelimiter = ':';
    private static int SaltSize { get; } = 16;
    private static int KeySize { get; } = 32;
    private static int Iterations { get; } = 100000;
    private static HashAlgorithmName Algorithm { get; } = HashAlgorithmName.SHA256;

    public string Hash(string secret)
    {
        var salt = RandomNumberGenerator.GetBytes(SaltSize);
        var key = Rfc2898DeriveBytes.Pbkdf2(
            secret,
            salt,
            Iterations,
            Algorithm,
            KeySize
        );
        return string.Join(
            SegmentDelimiter,
            Convert.ToHexString(key),
            Convert.ToHexString(salt)
        );
    }

    public bool Verify(string secret, string hash)
    {
        var segments = hash.Split(SegmentDelimiter);
        var key = Convert.FromHexString(segments[0]);
        var salt = Convert.FromHexString(segments[1]);
        var inputSecretKey = Rfc2898DeriveBytes.Pbkdf2(
            secret,
            salt,
            Iterations,
            Algorithm,
            key.Length
        );
        return key.SequenceEqual(inputSecretKey);
    }
}