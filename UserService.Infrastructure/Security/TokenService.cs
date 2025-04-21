using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using UserService.Application.Dtos.Responses;
using UserService.Application.Interfaces;
using UserService.Domain.Entities;
using UserService.Infrastructure.Configurations;
using UserService.Infrastructure.Persistence;

namespace UserService.Infrastructure.Security;

public class TokenService : ITokenService
{
    private readonly JwtSettings _jwtSettings;
    private readonly UserDbContext _userDbContext;

    public TokenService(
        IOptions<JwtSettings> jwtSettings,
        UserDbContext userDbContext)
    {
        _jwtSettings = jwtSettings.Value;
        _userDbContext = userDbContext;
    }

    public async Task<TokenResponseDto> GenerateTokens(User user)
    {
        var accessToken = GenerateAccessToken(user);
        var refreshToken = await GenerateRefreshToken(user);

        var tokens = new TokenResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
        };
        
        return tokens;
    }

    public async Task<TokenResponseDto> RefreshToken(string refreshToken)
    {
        var storedRefreshToken = await _userDbContext.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.Token == refreshToken && !rt.IsRevoked);
        
        if (storedRefreshToken == null || storedRefreshToken.ExpiresAt < DateTime.UtcNow)
        {
            throw new UnauthorizedAccessException("Refresh token is expired or invalid.");
        }
        
        var user = await _userDbContext.Users.FindAsync(storedRefreshToken.UserId);
        if (user == null)
        {
            throw new UnauthorizedAccessException("User not found.");
        }
        
        var accessToken = GenerateAccessToken(user);

        var tokenResponse = new TokenResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = storedRefreshToken.Token,
        };
        
        return tokenResponse;
    }

    private string GenerateAccessToken(User user)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Role, user.Role.ToString())
        };
        
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        
        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: DateTime.Now.AddMinutes(_jwtSettings.AccessTokenExpirationInMinutes),
            signingCredentials: credentials
        );
        
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private async Task<string> GenerateRefreshToken(User user)
    {
        var randomNumber = new byte[32];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomNumber);
        }
        
        var refreshToken = Convert.ToBase64String(randomNumber);

        var refreshTokenEntity = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Token = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationInDays),
            IsRevoked = false
        };
        
        await _userDbContext.RefreshTokens.AddAsync(refreshTokenEntity);
        await _userDbContext.SaveChangesAsync();
        
        return refreshToken;
    }
}