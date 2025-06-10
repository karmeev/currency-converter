using Bogus;
using Currency.Facades.Contracts.Requests;

namespace Currency.Facades.Tests.Fakes;

public static class FakeRequests
{
    public static LoginRequest GenerateLoginRequest()
    {
        var fake = new Faker<LoginRequest>()
            .RuleFor(x => x.Username, f => f.Person.UserName)
            .RuleFor(x => x.Password, f => f.Random.Word());
        return fake.Generate();
    }
}