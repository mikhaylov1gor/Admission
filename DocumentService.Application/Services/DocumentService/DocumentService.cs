using Contract.Application.Common;
using Contract.Application.Exceptions;
using Contract.Domain.Enums;
using DocumentService.Application.Dtos.Requests;
using DocumentService.Application.Dtos.Responses;
using DocumentService.Domain.Entities;
using DocumentService.Domain.IRepositories;

namespace DocumentService.Application.Services.DocumentService;

public class DocumentService : IDocumentService
{
    private readonly IPassportRepository _passportRepository;
    private readonly IEducationDocumentRepository _educationDocumentRepository;
    public DocumentService(
        IPassportRepository  passportRepository,
        IEducationDocumentRepository   educationDocumentRepository)
    {
        _passportRepository = passportRepository;
        _educationDocumentRepository = educationDocumentRepository;
    }
    
    public async Task<Result> UploadScanToDocument(Guid documentId, string scanFile, Guid userId)
    {
        return null;
    }

    public async Task<GenericResult<FileResponse>> DownloadDocumentScan(Guid documentId, Guid userId)
    {
        return null;
    }

    public async Task<Result> UpdateDocumentScan(Guid documentId, string scanFile, Guid userId)
    {
        return null;
    }

    public async Task<Result> DeleteDocumentScan(Guid documentId, Guid userId)
    {
        return null;
    }

    public async Task<GenericResult<Guid>> CreatePassport(CreatePassportDto dto, Guid userId)
    {
        var existingPassport = await _passportRepository.GetByUserIdAsync(userId);
        if (existingPassport != null)
        {
            throw new BadRequestException("Passport already exists");
        }
    
        var passport = new Passport
        {
            Id = Guid.NewGuid(),
            ApplicantId = userId,
            DocumentType = DocumentType.Passport,
            CreatedTime = DateTime.UtcNow,
            Serial = dto.Serial,
            Number = dto.Number,
            Hometown = dto.Hometown,
            WhenIssued = dto.WhenIssued,
            WhoIssued = dto.WhoIssued
        };
        await _passportRepository.AddAsync(passport);
        await _passportRepository.SaveChangesAsync();

        return GenericResult<Guid>.Success(passport.Id);
    }

    public async Task<GenericResult<PassportDto>> GetPassport(Guid userId)
    {
        var existingPassport = await _passportRepository.GetByUserIdAsync(userId);
        if (existingPassport == null)
        {
            throw new NotFoundException("Passport not found");
        }

        var passportDto = new PassportDto
        {
            Id = existingPassport.Id,
            CreatedTime = existingPassport.CreatedTime,
            ModifiedTime = existingPassport.ModifiedTime,
            Serial = existingPassport.Serial,
            Number = existingPassport.Number,
            Hometown = existingPassport.Hometown,
            WhenIssued = existingPassport.WhenIssued,
            WhoIssued = existingPassport.WhoIssued
        };
        
        return GenericResult<PassportDto>.Success(passportDto);
    }

    public async Task<Result> EditPassport(EditPassportDto dto, Guid userId)
    {
        var existingPassport = await _passportRepository.GetByUserIdAsync(userId);
        if (existingPassport == null)
        {
            throw new NotFoundException("Passport not found");
        }
        
        existingPassport.Serial = dto.Serial;
        existingPassport.Number = dto.Number;
        existingPassport.Hometown = dto.Hometown;
        existingPassport.WhenIssued = dto.WhenIssued;
        existingPassport.WhoIssued = dto.WhoIssued;
        
        await _passportRepository.SaveChangesAsync();
        
        return Result.Success();
    }

    public async Task<Result> DeletePassport(Guid userId)
    {
        var existingPassport = await _passportRepository.GetByUserIdAsync(userId);
        if (existingPassport == null)
        {
            throw new NotFoundException("Passport not found");
        }
        
        await _passportRepository.DeletePassportAsync(userId);
        return Result.Success();
    }

    public async Task<GenericResult<Guid>> CreateEducationDocument(CreateEducationDocumentDto dto, Guid userId)
    {
        var existingEducationDocument = await _passportRepository.GetByUserIdAsync(userId);
        if (existingEducationDocument != null)
        {
            throw new BadRequestException("Education document already exists");
        }

        var educationDocument = new EducationDocument
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            EducationDocumentTypeId = dto.EducationDocumentTypeId,
            ApplicantId = userId,
            DocumentType = DocumentType.Passport,
            CreatedTime = DateTime.UtcNow,
        };
        
        await _educationDocumentRepository.AddAsync(educationDocument);
        await _educationDocumentRepository.SaveChangesAsync();
        
        return GenericResult<Guid>.Success(educationDocument.Id);
    }

    public async Task<GenericResult<EducationDocumentDto>> GetEducationDocument(Guid userId)
    {
        var existingEducationDocument = await _educationDocumentRepository.GetByUserIdAsync(userId);
        if (existingEducationDocument == null)
        {
            throw new NotFoundException("Education document not found");
        }

        var educationDocumentDto = new EducationDocumentDto
        {
            Id = existingEducationDocument.Id,
            CreatedTime = existingEducationDocument.CreatedTime,
            ModifiedTime = existingEducationDocument.ModifiedTime,
            EducationDocumentType = new EducationDocumentTypeDto
            {
                Id = existingEducationDocument.EducationDocumentType.Id,
                Name = existingEducationDocument.EducationDocumentType.Name,
                EducationLevel = new  EducationLevelDto
                {
                    Id = existingEducationDocument.EducationDocumentType.EducationLevel.Id,
                    Name = existingEducationDocument.EducationDocumentType.EducationLevel.Name,
                },
            },
            Name = existingEducationDocument.Name,
        };
        
        return GenericResult<EducationDocumentDto>.Success(educationDocumentDto);
    }

    public async Task<Result> EditEducationDocument(EditEducationDocumentDto dto, Guid userId)
    {
        var existingEducationDocument = await _educationDocumentRepository.GetByUserIdAsync(userId);
        if (existingEducationDocument == null)
        {
            throw new NotFoundException("Education document not found");
        }
        
        existingEducationDocument.Name = dto.Name;
        existingEducationDocument.EducationDocumentTypeId = dto.EducationDocumentTypeId;
        
        await _educationDocumentRepository.SaveChangesAsync();
        return Result.Success();
    }

    public async Task<Result> DeleteEducationDocument(Guid userId)
    {
        var existingEducationDocument = await _educationDocumentRepository.GetByUserIdAsync(userId);
        if (existingEducationDocument == null)
        {
            throw new NotFoundException("Education document not found");
        }
        
        await _educationDocumentRepository.DeleteEducationDocumentAsync(userId);
        return Result.Success();
    }
}