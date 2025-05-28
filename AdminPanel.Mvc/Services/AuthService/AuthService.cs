using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using AdminPanel.Mvc.Models.Account;
using AdminPanel.Mvc.Services.AuthService;
using AdminPanel.Mvc.Services.UserServiceClient;
using Microsoft.AspNetCore.Authentication.Cookies;

public class AuthService : IAuthService
{
    private readonly IUserServiceClient _userServiceClient;
    private readonly IHttpContextAccessor _httpContextAccessor;
    
    public AuthService(
        IUserServiceClient userServiceClient,
        IHttpContextAccessor httpContextAccessor)
    {
        _userServiceClient = userServiceClient;
        _httpContextAccessor = httpContextAccessor;
    }
    
    public async Task<ClaimsIdentity> Login(LoginViewModel loginViewModel)
    {
        var response = await _userServiceClient.Login(loginViewModel.Email, loginViewModel.Password);
        
        if (string.IsNullOrEmpty(response.accessToken))
        {
            return null;
        }
        
        SetTokensInCookies(response.accessToken, response.refreshToken, loginViewModel.RememberMe);

        return CreateClaimsIdentity(response.accessToken);
    }

    private ClaimsIdentity CreateClaimsIdentity(string accessToken)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadJwtToken(accessToken);
        var claims = jwtToken.Claims.ToList();
        
        claims.Add(new Claim("AccessToken", accessToken));

        return new ClaimsIdentity(
            claims,
            CookieAuthenticationDefaults.AuthenticationScheme,
            ClaimTypes.Name,
            ClaimTypes.Role);
    }

    private void SetTokensInCookies(string accessToken, string refreshToken, bool rememberMe)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true, 
            SameSite = SameSiteMode.Strict,
            Expires = rememberMe ? DateTime.Now.AddDays(30) : null
        };

        _httpContextAccessor.HttpContext.Response.Cookies.Append("AccessToken", accessToken, cookieOptions);
        
        if (!string.IsNullOrEmpty(refreshToken))
        {
            _httpContextAccessor.HttpContext.Response.Cookies.Append("RefreshToken", refreshToken, cookieOptions);
        }
    }

    public async Task<bool> RefreshTokens()
    {
        var refreshToken = _httpContextAccessor.HttpContext.Request.Cookies["RefreshToken"];
        if (string.IsNullOrEmpty(refreshToken))
            return false;

        var response = await _userServiceClient.RefreshTokens(refreshToken);
        if (string.IsNullOrEmpty(response.accessToken))
            return false;

        SetTokensInCookies(response.accessToken, response.refreshToken, true);
        return true;
    }
}