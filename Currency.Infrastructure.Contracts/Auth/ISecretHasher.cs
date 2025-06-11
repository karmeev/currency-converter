namespace Currency.Infrastructure.Contracts.Auth;

public interface ISecretHasher
{
    string Hash(string secret);
    bool Verify(string secret, string hash);
}