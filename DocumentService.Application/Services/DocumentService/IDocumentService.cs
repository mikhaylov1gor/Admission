using Contract.Application.Common;
using DocumentService.Application.Dtos.Requests;
using DocumentService.Application.Dtos.Responses;

namespace DocumentService.Application.Services.DocumentService;

public interface IDocumentService
{
    Task<Guid> CreatePassport(CreatePassportDto dto, Guid userId);
    Task<PassportDto> GetPassport(Guid userId);
    Task<Result> EditPassport(EditPassportDto passport, Guid userId);
    Task<Result> DeletePassport(Guid userId);
    
    Task<Guid> CreateEducationDocument(CreateEducationDocumentDto dto, Guid userId);
    Task<EducationDocumentDto> GetEducationDocument(Guid userId);
    Task<Result> EditEducationDocument(EditEducationDocumentDto dto, Guid userId);
    Task<Result> DeleteEducationDocument(Guid userId);
    
}