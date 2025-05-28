using System.Security.Claims;
using AdminPanel.Mvc.Models.Account;

namespace AdminPanel.Mvc.Services.AuthService;

public interface IAuthService
{
    Task<ClaimsIdentity> Login(LoginViewModel loginViewModel);
    Task<bool> RefreshTokens();
}