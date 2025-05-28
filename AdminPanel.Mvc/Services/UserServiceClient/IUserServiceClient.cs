using AdminPanel.Mvc.Models.Dtos;

namespace AdminPanel.Mvc.Services.UserServiceClient;

public interface IUserServiceClient
{
    Task<AuthResponse> Login(string email, string password);
    Task<AuthResponse> RefreshTokens(string refreshToken);
    Task<ChangePasswordResult> ChangePassword(string currentPassword, string newPassword);
}