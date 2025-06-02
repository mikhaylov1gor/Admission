namespace AdmissionService.Application.Services.UserServiceClient;

public interface IUserServiceClient
{
    Task<string> GetUserEmail(Guid userId);
}