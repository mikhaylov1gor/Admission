using DocumentService.Application.Dtos.Responses;

namespace AdminPanel.Mvc.Services.DocumentServiceClient;

public interface IDocumentServiceClient
{
    Task<PassportDto> GetPassport(Guid applicantId);
    Task<EducationDocumentDto> GetEducationDocument(Guid applicantId);
    Task<DownloadFileDto> DownloadScan(Guid scanId, Guid applicantId);
}