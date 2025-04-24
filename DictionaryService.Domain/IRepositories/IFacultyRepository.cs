using DictionaryService.Domain.Entities;

namespace DictionaryService.Domain.IRepositories;

public interface IFacultyRepository
{
    Task<List<Faculty>> GetAllAsync();
}