using System.Security.Claims;
using AdminPanel.Mvc.Services.AuthService;

namespace AdminPanel.Mvc.Middlewaries;

public class TokenRefreshMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<TokenRefreshMiddleware> _logger;

    public TokenRefreshMiddleware(
        RequestDelegate next,
        ILogger<TokenRefreshMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context, IAuthService authService)
    {
        try
        {
            if (ShouldRefreshToken(context))
            {
                _logger.LogInformation("Attempting to refresh tokens");
                var refreshed = await authService.RefreshTokens();
                if (refreshed)
                {
                    _logger.LogInformation("Tokens refreshed successfully");
                }
                else
                {
                    _logger.LogWarning("Failed to refresh tokens");
                }
            }

            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in TokenRefreshMiddleware");
            throw;
        }
    }

    private bool ShouldRefreshToken(HttpContext context)
    {
        if (context.Request.Path.StartsWithSegments("/Account/Login") ||
            context.Request.Path.StartsWithSegments("/Account/Logout"))
        {
            return false;
        }

        if (context.User.Identity?.IsAuthenticated != true)
        {
            return false;
        }

        var tokenExp = context.User.FindFirstValue("exp");
        if (!long.TryParse(tokenExp, out var expUnixTime))
        {
            return false;
        }

        var expTime = DateTimeOffset.FromUnixTimeSeconds(expUnixTime);
        return expTime < DateTimeOffset.UtcNow.AddMinutes(5);
    }
}