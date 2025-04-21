using UserService.Domain.Entities;

namespace UserService.Domain.IRepositories;

public interface IManagerRepository
{
    Task<Manager?> GetByIdAsync(Guid id);
    Task<List<Manager>> GetAllAsync();
    Task AddAsync(Manager manager);
    Task SaveChangesAsync();
}