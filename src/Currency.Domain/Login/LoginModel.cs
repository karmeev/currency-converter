namespace Currency.Domain.Login;

public struct LoginModel(string username, string password)
{
    public readonly string Username = username;
    public string Password = password;
}