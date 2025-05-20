using DocumentService.Domain.Entities;

namespace DocumentService.Domain.IRepositories;

public interface IEducationDocumentTypeRepository
{
    Task<EducationDocumentType?> GetEducationDocumentTypeByIdAsync(Guid id);
    Task AddAsync(EducationDocumentType file);
}