using Contract.Application.Common;
using UserService.Application.Dtos.Requests;
using UserService.Application.Dtos.Responses;

namespace UserService.Application.Services.ApplicantService;

public interface IApplicantService
{
    Task<GenericResult<ApplicantDto>> GetProfile(Guid userId);
    Task<Result> EditProfile(EditUserDto dto, Guid userId);
    Task<Result> ResetPassword(ResetPasswordDto dto, Guid userId);
}