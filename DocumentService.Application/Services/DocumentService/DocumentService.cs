using Contract.Application.Common;
using Contract.Application.Exceptions;
using Contract.Domain.Enums;
using DocumentService.Application.Dtos.Requests;
using DocumentService.Application.Dtos.Responses;
using DocumentService.Application.Services.DictionaryServiceClient;
using DocumentService.Domain.Entities;
using DocumentService.Domain.IRepositories;

namespace DocumentService.Application.Services.DocumentService;

public class DocumentService : IDocumentService
{
    private readonly IPassportRepository _passportRepository;
    private readonly IEducationDocumentTypeRepository _educationDocumentTypeRepository;
    private readonly IEducationDocumentRepository _educationDocumentRepository;
    private readonly IDictionaryServiceClient _dictionaryServiceClient;
    public DocumentService(
        IPassportRepository  passportRepository,
        IEducationDocumentRepository   educationDocumentRepository,
        IEducationDocumentTypeRepository educationDocumentTypeRepository,
        IDictionaryServiceClient dictionaryServiceClient)
    {
        _passportRepository = passportRepository;
        _educationDocumentRepository = educationDocumentRepository;
        _educationDocumentTypeRepository = educationDocumentTypeRepository;
        _dictionaryServiceClient = dictionaryServiceClient;
    }
    
    public async Task<Guid> CreatePassport(CreatePassportDto dto, Guid userId)
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

        return passport.Id;
    }

    public async Task<PassportDto> GetPassport(Guid userId)
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
        
        var docFiles = existingPassport.Files?
            .Select(f => new FileDto
            {
                Id = f.Id,
                FileName = $"{f.FileName}.{f.Extension}",
            })
            .ToList() ?? new List<FileDto>();
        
        passportDto.Files = docFiles;

        return passportDto;
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
        existingPassport.ModifiedTime = DateTime.UtcNow;
        
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

    public async Task<Guid> CreateEducationDocument(CreateEducationDocumentDto dto, Guid userId)
    {
        var existingEducationDocument = await _educationDocumentRepository.GetByUserIdAsync(userId);
        if (existingEducationDocument != null)
        {
            throw new BadRequestException("Education document already exists");
        }

        var externalEducationDocumentTypes = await _dictionaryServiceClient.GetEducationDocumentTypesAsync();
        
        var documentType = externalEducationDocumentTypes.FirstOrDefault(d => d.Id == dto.EducationDocumentTypeId);
        if (documentType == null)
        {
            throw new BadRequestException("Invalid EducationDocumentTypeId");
        }
        
        var existingType = await _educationDocumentTypeRepository.GetEducationDocumentTypeByIdAsync(documentType.Id);

        EducationDocumentType typeEntity;
        if (existingType != null)
        {
            typeEntity = existingType;
        }
        else
        {
            typeEntity = new EducationDocumentType
            {
                Id = documentType.Id,
                Name = documentType.Name,
                EducationLevel = new EducationLevel
                {
                    Id = documentType.EducationLevel.Id,
                    Name = documentType.EducationLevel.Name
                }
            };
        }


        var educationDocument = new EducationDocument
        {
            Id = Guid.NewGuid(),
            ApplicantId = userId,
            DocumentType = DocumentType.EducationDocument,
            CreatedTime = DateTime.UtcNow,
            
            Name = dto.Name,
            EducationDocumentTypeId = dto.EducationDocumentTypeId,
            EducationDocumentType = typeEntity,
        };
        
        await _educationDocumentRepository.AddAsync(educationDocument);
        await _educationDocumentRepository.SaveChangesAsync();

        return educationDocument.Id;
    }

    public async Task<EducationDocumentDto> GetEducationDocument(Guid userId)
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
        
        var docFiles = existingEducationDocument.Files?
            .Select(f => new FileDto
            {
                Id = f.Id,
                FileName = $"{f.FileName}.{f.Extension}",
            })
            .ToList() ?? new List<FileDto>();
        
        educationDocumentDto.Files = docFiles;

        return educationDocumentDto;
    }

    public async Task<Result> EditEducationDocument(EditEducationDocumentDto dto, Guid userId)
    {
        var existingEducationDocument = await _educationDocumentRepository.GetByUserIdAsync(userId);
        if (existingEducationDocument == null)
        {
            throw new NotFoundException("Education document not found");
        }
        
        var externalEducationDocumentTypes = await _dictionaryServiceClient.GetEducationDocumentTypesAsync();
        
        var documentType = externalEducationDocumentTypes.FirstOrDefault(d => d.Id == dto.EducationDocumentTypeId);
        if (documentType == null)
        {
            throw new BadRequestException("Invalid EducationDocumentTypeId");
        }
        
        var existingType = await _educationDocumentTypeRepository.GetEducationDocumentTypeByIdAsync(documentType.Id);

        EducationDocumentType typeEntity;
        if (existingType != null)
        {
            typeEntity = existingType;
        }
        else
        {
            typeEntity = new EducationDocumentType
            {
                Id = documentType.Id,
                Name = documentType.Name,
                EducationLevel = new EducationLevel
                {
                    Id = documentType.EducationLevel.Id,
                    Name = documentType.EducationLevel.Name
                }
            };
            
            await _educationDocumentTypeRepository.AddAsync(typeEntity);
        }
        
        
        existingEducationDocument.Name = dto.Name;
        existingEducationDocument.ModifiedTime = DateTime.UtcNow;
        existingEducationDocument.EducationDocumentTypeId = dto.EducationDocumentTypeId;
        existingEducationDocument.EducationDocumentType = typeEntity;
        
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