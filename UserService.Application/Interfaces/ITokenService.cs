using UserService.Application.Dtos.Responses;
using UserService.Domain.Entities;

namespace UserService.Application.Interfaces;

public interface ITokenService
{
    TokenResponseDto GenerateTokens(User user);
}