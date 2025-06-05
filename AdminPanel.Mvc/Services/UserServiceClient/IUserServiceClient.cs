using AdminPanel.Mvc.Models.Dtos;
using UserService.Application.Dtos.Responses;

namespace AdminPanel.Mvc.Services.UserServiceClient;

public interface IUserServiceClient
{
    Task<AuthResponse> Login(string email, string password);
    Task<AuthResponse> RefreshTokens(string refreshToken);
    Task<ChangePasswordResult> ChangePassword(string currentPassword, string newPassword);
    Task<List<ManagerDto>> GetAllManagers(bool isAllManagers);
    Task<List<ApplicantDto>> GetAllApplicants();
    Task<bool> AssignRole(Guid userId, string role);
    
    Task<ApplicantDto> GetApplicant(Guid applicantId);
}