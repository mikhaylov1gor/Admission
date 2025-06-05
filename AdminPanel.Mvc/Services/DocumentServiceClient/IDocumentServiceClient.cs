using DocumentService.Application.Dtos.Requests;
using DocumentService.Application.Dtos.Responses;

namespace AdminPanel.Mvc.Services.DocumentServiceClient;

public interface IDocumentServiceClient
{
    Task<PassportDto> GetPassport(Guid applicantId);
    Task<EducationDocumentDto> GetEducationDocument(Guid applicantId);
    Task<DownloadFileDto> DownloadScan(Guid scanId, Guid applicantId);
    Task<bool> EditPassport(EditPassportDto dto, Guid applicantId);
    Task<bool> EditEducationDocument(EditEducationDocumentDto dto, Guid applicantId);
    Task<bool> DeleteFile(Guid scanId, Guid applicantId);
    Task<bool> UploadFile(CreateScanDto dto, Guid documentId, Guid userId);
}