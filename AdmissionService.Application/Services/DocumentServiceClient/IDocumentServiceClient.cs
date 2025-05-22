namespace AdmissionService.Application.Services.DocumentServiceClient;

public interface IDocumentServiceClient
{
    Task<string> GetEducationDocumentLevelName(Guid programId);
}