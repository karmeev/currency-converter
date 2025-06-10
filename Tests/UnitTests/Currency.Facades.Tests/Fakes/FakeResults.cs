using Bogus;

namespace Currency.Facades.Tests.Fakes;

public static class FakeResults
{
    public static string GenerateRefreshToken()
    {
        return new Faker().Random.Guid().ToString();
    }
    
    public static string GenerateAccessToken()
    {
        return new Faker().Random.Guid().ToString();
    }
}