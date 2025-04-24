using DictionaryService.Domain.Entities;

namespace DictionaryService.Domain.IRepositories;

public interface IDocumentTypeRepository
{
    Task<List<EducationDocumentType>> GetAllAsync();
}