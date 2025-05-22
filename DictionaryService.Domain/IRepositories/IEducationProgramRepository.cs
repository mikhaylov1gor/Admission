using DictionaryService.Domain.Entities;

namespace DictionaryService.Domain.IRepositories;

public interface IEducationProgramRepository
{
    Task<List<EducationProgram>> GetProgramsByParameters(
        string? facultyName,
        string? educationLevelName,
        string? educationForm,
        string? language,
        string? name);
    
    Task<EducationProgram?> GetProgramById(Guid programId);
}