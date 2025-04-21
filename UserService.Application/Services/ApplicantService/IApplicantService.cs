using Contract.Application.Common;
using UserService.Application.Dtos.Requests;
using UserService.Application.Dtos.Responses;

namespace UserService.Application.Services.UserService;

public interface IApplicantService
{
    Task<GenericResult<ApplicantDto>> GetProfile();
    Task<Result> EditProfile(EditUserDto dto);
    Task<Result> ResetPassword(ResetPasswordDto dto);
}