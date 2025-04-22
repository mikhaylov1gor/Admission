using UserService.Domain.Entities;

namespace UserService.Domain.IRepositories;

public interface IRefreshTokenRepository
{
    Task<RefreshToken?> GetByUserIdAsync(Guid id);
    Task<List<RefreshToken>> GetAllByUserIdAsync(Guid id);
    Task AddAsync(RefreshToken token);
    Task SaveChangesAsync();
    Task DeleteAsync(RefreshToken token);
}