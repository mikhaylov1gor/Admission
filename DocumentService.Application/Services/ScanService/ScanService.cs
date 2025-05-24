using Contract.Application.Common;
using Contract.Application.Exceptions;
using DocumentService.Application.Dtos.Requests;
using DocumentService.Application.Dtos.Responses;
using DocumentService.Application.Services.AdmissionServiceClient;
using DocumentService.Application.Services.DocumentService;
using DocumentService.Domain.IRepositories;
using File = DocumentService.Domain.Entities.File;

namespace DocumentService.Application.Services.ScanService;

public class ScanService : IScanService
{
    private readonly IDocumentRepository _documentRepository;
    private readonly IAdmissionServiceClient _admissionServiceClient;

    public ScanService(
        IDocumentRepository  documentRepository,
        IAdmissionServiceClient admissionServiceClient)
    {
        _documentRepository = documentRepository;
        _admissionServiceClient = admissionServiceClient;
    }
    
    public async Task<Guid> UploadScanToDocument(Guid documentId, CreateScanDto dto, Guid userId)
    {
        if (!await _admissionServiceClient.IsAdmissionOpen())
        {
            throw new ForbiddenAccessException("University admission is not currently available");
        }

        var document = await _documentRepository.GetDocumentByUserIdAndDocumentIdAsync(documentId, userId);

        if (document == null)
        {
            throw new NotFoundException("Document not found");
        }
        
        if (dto.File == null || dto.File.Length == 0)
        {
            throw new BadRequestException("File is empty or missing");
        }
        
        byte[] fileBytes;
        using (var stream = new MemoryStream())
        {
            await dto.File.CopyToAsync(stream);
            fileBytes = stream.ToArray();   
        }

        var newFile = new File
        {
            Id = Guid.NewGuid(),
            DocumentId = document.Id,
            Document = document,
            FileName = Path.GetFileNameWithoutExtension(dto.File.FileName),
            Extension = Path.GetExtension(dto.File.FileName).TrimStart('.'),
            Data = fileBytes,
            FileSize = fileBytes.Length,
        };

        await _documentRepository.AddFileAsync(newFile);

        return newFile.Id;
    }

    public async Task<DownloadFileDto> DownloadDocumentScan(Guid scanId, Guid userId)
    {
        var file = await _documentRepository.GetFileByScanIdAndUserIdAsync(scanId, userId);
    
        if (file == null)
        {
           throw new NotFoundException("Scan not found");
        }

        return new DownloadFileDto()
        {
            Data = file.Data,
            FileName = $"{file.FileName}.{file.Extension}",
            ContentType = GetContentType(file.Extension)
        };

    }

    public async Task<Result> UpdateDocumentScan(Guid scanId, UpdateScanDto dto, Guid userId)
    {
        if (!await _admissionServiceClient.IsAdmissionOpen())
        {
            throw new ForbiddenAccessException("University admission is not currently available");
        }

        
        var file = await _documentRepository.GetFileByScanIdAndUserIdAsync(scanId, userId);
    
        if (file == null)
        {
            throw new NotFoundException("Scan not found");
        }
        
        byte[] newFileBytes;
        using (var stream = new MemoryStream())
        {
            await dto.NewFile.CopyToAsync(stream);
            newFileBytes = stream.ToArray();
        }

        file.Data = newFileBytes;
        file.FileSize = newFileBytes.Length;
        file.FileName = Path.GetFileNameWithoutExtension(dto.NewFile.FileName);
        file.Extension = Path.GetExtension(dto.NewFile.FileName).TrimStart('.');
        
        await _documentRepository.SaveChangesAsync();
        return Result.Success();
    }

    public async Task<Result> DeleteDocumentScan(Guid scanId, Guid userId)
    {
        if (!await _admissionServiceClient.IsAdmissionOpen())
        {
            throw new ForbiddenAccessException("University admission is not currently available");
        }

        
        var file = await _documentRepository.GetFileByScanIdAndUserIdAsync(scanId, userId);
    
        if (file == null)
        {
            throw new NotFoundException("Scan not found");
        }

        await _documentRepository.DeleteFileAsync(file);
        return Result.Success();
    }
    
    private string GetContentType(string extension)
    {
        return extension.ToLower() switch
        {
            "pdf" => "application/pdf",
            "jpg" => "image/jpeg",
            "jpeg" => "image/jpeg",
            "png" => "image/png",
            _ => "application/octet-stream"
        };
    }
}