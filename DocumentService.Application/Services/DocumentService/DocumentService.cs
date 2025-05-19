using Contract.Application.Common;
using DocumentService.Application.Dtos.Requests;
using DocumentService.Application.Dtos.Responses;

namespace DocumentService.Application.Services.DocumentService;

public class DocumentService : IDocumentService
{

    public async Task<Result> UploadScanToDocument(Guid documentId, string scanFile)
    {
        return null;
    }

    public async Task<GenericResult<FileResponse>> DownloadDocumentScan(Guid documentId)
    {
        return null;
    }

    public async Task<Result> UpdateDocumentScan(Guid documentId, string scanFile)
    {
        return null;
    }

    public async Task<Result> DeleteDocumentScan(Guid documentId)
    {
        return null;
    }

    public async Task<Result> CreatePassport(CreatePassportDto dto)
    {
        return null;
    }

    public async Task<GenericResult<PassportDto>> GetPassport()
    {
        return null;
    }

    public async Task<Result> EditPassport(EditPassportDto dto)
    {
        return null;
    }

    public async Task<Result> DeletePassport()
    {
        return null;
    }

    public async Task<Result> CreateEducationDocument(CreateEducationDocumentDto dto)
    {
        return null;
    }

    public async Task<GenericResult<EducationDocumentDto>> GetEducationDocument()
    {
        return null;
    }

    public async Task<Result> EditEducationDocument(EditEducationDocumentDto dto)
    {
        return null;
    }

    public async Task<Result> DeleteEducationDocument()
    {
        return null;
    }
}