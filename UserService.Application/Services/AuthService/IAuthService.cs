using Contract.Application.Common;
using UserService.Application.Dtos.Requests;
using UserService.Application.Dtos.Responses;

namespace UserService.Application.Services.AuthService;

public interface IAuthService
{
    Task<GenericResult<TokenResponseDto>> RegisterApplicant(RegisterUserDto dto);
    Task<GenericResult<TokenResponseDto>> Login(LoginCredentialsDto dto);
    Task<GenericResult<TokenResponseDto>> RefreshTokens();
    Task<Result> Logout();
}