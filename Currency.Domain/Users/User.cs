namespace Currency.Domain.Users;

public class User
{
    public string Id { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public UserRole Role { get; set; }
}