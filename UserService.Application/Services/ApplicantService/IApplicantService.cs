using Contract.Application.Common;
using Contract.Domain.Enums;
using UserService.Application.Dtos.Requests;
using UserService.Application.Dtos.Responses;

namespace UserService.Application.Services.ApplicantService;

public interface IApplicantService
{
    Task<GenericResult<ApplicantDto>> GetProfile(Guid userId);
    Task<Result> EditProfile(EditUserDto dto, Guid userId);
    Task<Result> ResetPassword(ResetPasswordDto dto, Guid userId);

    Task<Role> GetRoleByUserId(Guid userId);
    
    Task<List<ApplicantDto>> GetAllApplicants();
    Task<List<ManagerDto>> GetAllManagers();
    
    Task AssignRoleTo(Guid userId, Role role);
}