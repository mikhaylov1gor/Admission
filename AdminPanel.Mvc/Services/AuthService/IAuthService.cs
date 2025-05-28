using System.Security.Claims;
using AdminPanel.Mvc.Models.Account;
using AdminPanel.Mvc.Models.Dtos;

namespace AdminPanel.Mvc.Services.AuthService;

public interface IAuthService
{
    Task<ClaimsIdentity> Login(LoginViewModel loginViewModel);
    Task<bool> RefreshTokens();
    Task<ChangePasswordResult> ChangePassword(string currentPassword, string newPassword);
}