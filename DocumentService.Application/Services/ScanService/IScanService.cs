using Contract.Application.Common;
using DocumentService.Application.Dtos.Requests;
using DocumentService.Application.Dtos.Responses;

namespace DocumentService.Application.Services.ScanService;

public interface IScanService
{
    Task<Guid> UploadScanToDocument(Guid documentId, CreateScanDto dto, Guid userId);
    Task<DownloadFileDto> DownloadDocumentScan(Guid scanId, Guid userId);
    Task<Result> UpdateDocumentScan(Guid scanId, UpdateScanDto dto, Guid userId);
    Task<Result> DeleteDocumentScan(Guid scanId, Guid userId);
}