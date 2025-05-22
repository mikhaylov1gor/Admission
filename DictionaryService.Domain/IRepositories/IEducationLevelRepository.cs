using DictionaryService.Domain.Entities;

namespace DictionaryService.Domain.IRepositories;

public interface IEducationLevelRepository
{
    Task<List<EducationLevel>> GetAllAsync();
    Task<EducationLevel> GetByIdAsync(Guid educationLevelId);
}