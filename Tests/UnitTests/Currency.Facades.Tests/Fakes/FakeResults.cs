using Bogus;
using Currency.Domain.Login;

namespace Currency.Facades.Tests.Fakes;

public static class FakeResults
{
    public static Tokens GenerateFakeTokens()
    {
        var fakeToken = new Tokens
        {
            AccessToken = new Faker().Random.Hash(),
            RefreshToken = new Faker().Random.Hash()
        };

        return fakeToken;
    }
}