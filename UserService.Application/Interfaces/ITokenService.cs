using UserService.Application.Dtos.Responses;
using UserService.Domain.Entities;

namespace UserService.Application.Interfaces;

public interface ITokenService
{
    Task<TokenResponseDto> GenerateTokens(User user);
    Task<TokenResponseDto> RefreshToken(string refreshToken);
    Task DeleteTokens(List<RefreshToken> refreshTokens);
}