namespace AdminPanel.Mvc.Services.AuthService;

public interface IAuthServiceClient
{
    Task<AuthResponse> LoginAsync(string email, string password);
}