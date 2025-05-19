using DocumentService.Domain.Entities;

namespace DocumentService.Domain.IRepositories;

public interface IPassportRepository
{
    Task<Passport?> GetByIdAsync(Guid id);
    Task<Passport?> GetByUserIdAsync(Guid userId);
    Task<List<Passport>> GetAllAsync();
    
    Task DeletePassportAsync(Guid userId);
    Task AddAsync(Passport passport);
    Task SaveChangesAsync();
}