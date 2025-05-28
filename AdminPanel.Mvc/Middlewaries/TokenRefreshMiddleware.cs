using System.Security.Claims;
using AdminPanel.Mvc.Services.AuthService;

namespace AdminPanel.Mvc.Middlewaries;

public class TokenRefreshMiddleware
{
    private readonly RequestDelegate _next;

    public TokenRefreshMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context, IAuthService authService)
    {
        await _next(context);
        
        if (context.User.Identity.IsAuthenticated && 
            context.Request.Path != "/Account/Login" &&
            context.Request.Path != "/Account/Logout")
        {
            var tokenExp = context.User.FindFirstValue("exp");
            if (long.TryParse(tokenExp, out var expUnixTime))
            {
                var expTime = DateTimeOffset.FromUnixTimeSeconds(expUnixTime);
                if (expTime < DateTimeOffset.UtcNow.AddMinutes(5)) 
                {
                    await authService.RefreshTokens();
                }
            }
        }
    }
}