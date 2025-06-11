using Currency.Domain.Users;

namespace Currency.Api.Models;

public class UsersModel
{
    public IEnumerable<User> Users { get; set; }
}