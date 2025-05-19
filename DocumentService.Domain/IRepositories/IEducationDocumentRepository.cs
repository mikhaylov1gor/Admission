using DocumentService.Domain.Entities;

namespace DocumentService.Domain.IRepositories;

public interface IEducationDocumentRepository
{
    Task<EducationDocument?> GetByIdAsync(Guid id);
    Task<EducationDocument?> GetByUserIdAsync(Guid userId);
    Task<List<EducationDocument>> GetAllAsync();
    
    Task DeleteEducationDocumentAsync(Guid userId);
    Task AddAsync(EducationDocument educationDocument);
    Task SaveChangesAsync();
}