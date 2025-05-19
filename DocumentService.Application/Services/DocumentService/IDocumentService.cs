using Contract.Application.Common;
using DocumentService.Application.Dtos.Requests;
using DocumentService.Application.Dtos.Responses;

namespace DocumentService.Application.Services.DocumentService;

public interface IDocumentService
{
    Task<Result> UploadScanToDocument(Guid documentId, string scanFile);
    Task<GenericResult<FileResponse>> DownloadDocumentScan(Guid documentId);
    Task<Result> UpdateDocumentScan (Guid documentId, string scanFile);
    Task<Result> DeleteDocumentScan(Guid documentId);
    
    Task<Result> CreatePassport(CreatePassportDto passport);
    Task<GenericResult<PassportDto>> GetPassport();
    Task<Result> EditPassport(EditPassportDto passport);
    Task<Result> DeletePassport();
    
    Task<Result> CreateEducationDocument(CreateEducationDocumentDto dto);
    Task<GenericResult<EducationDocumentDto>> GetEducationDocument();
    Task<Result> EditEducationDocument(EditEducationDocumentDto dto);
    Task<Result> DeleteEducationDocument();
    
}