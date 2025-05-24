namespace DocumentService.Application.Services.AdmissionServiceClient;

public interface IAdmissionServiceClient
{
    Task<bool> IsAdmissionOpen();
}