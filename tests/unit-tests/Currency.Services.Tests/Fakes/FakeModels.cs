using Bogus;
using Currency.Domain.Users;

namespace Currency.Services.Tests.Fakes;

public class FakeModels
{
    public static User GenerateFakeUser()
    {
        return new Faker<User>()
            .RuleFor(x => x.Id, f => f.Random.Number().ToString())
            .RuleFor(x => x.Username, f => f.Person.UserName)
            .RuleFor(x => x.Password, f => f.Random.Hash())
            .RuleFor(x => x.Role, f => f.Random.Enum<UserRole>());
    }
}