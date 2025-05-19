using Contract.Application.Common;
using DocumentService.Application.Dtos.Requests;
using DocumentService.Application.Dtos.Responses;

namespace DocumentService.Application.Services.DocumentService;

public interface IDocumentService
{
    Task<Result> UploadScanToDocument(Guid documentId, string scanFile, Guid userId);
    Task<GenericResult<FileResponse>> DownloadDocumentScan(Guid documentId, Guid userId);
    Task<Result> UpdateDocumentScan (Guid documentId, string scanFile, Guid userId);
    Task<Result> DeleteDocumentScan(Guid documentId, Guid userId);
    
    Task<GenericResult<Guid>> CreatePassport(CreatePassportDto passport, Guid userId);
    Task<GenericResult<PassportDto>> GetPassport(Guid userId);
    Task<Result> EditPassport(EditPassportDto passport, Guid userId);
    Task<Result> DeletePassport(Guid userId);
    
    Task<GenericResult<Guid>> CreateEducationDocument(CreateEducationDocumentDto dto, Guid userId);
    Task<GenericResult<EducationDocumentDto>> GetEducationDocument(Guid userId);
    Task<Result> EditEducationDocument(EditEducationDocumentDto dto, Guid userId);
    Task<Result> DeleteEducationDocument(Guid userId);
    
}