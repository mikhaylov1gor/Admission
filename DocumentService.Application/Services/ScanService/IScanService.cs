using Contract.Application.Common;
using DocumentService.Application.Dtos.Requests;
using DocumentService.Application.Dtos.Responses;

namespace DocumentService.Application.Services.ScanService;

public interface IScanService
{
    Task<GenericResult<Guid>> UploadScanToDocument(Guid documentId, CreateScanDto dto, Guid userId);
    Task<GenericResult<FileDto>> DownloadDocumentScan(Guid scanId, Guid userId);
    Task<Result> UpdateDocumentScan(Guid scanId, UpdateScanDto dto, Guid userId);
    Task<Result> DeleteDocumentScan(Guid scanId, Guid userId);
}