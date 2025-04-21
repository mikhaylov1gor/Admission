using Microsoft.EntityFrameworkCore;
using UserService.Domain.Entities;
using UserService.Domain.IRepositories;
using UserService.Infrastructure.Persistence;

namespace UserService.Infrastructure.Repositories;

public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly UserDbContext _context;

    public RefreshTokenRepository(UserDbContext context)
    {
        _context = context;
    }

    public async Task<RefreshToken?> GetByUserIdAsync(Guid userId)
    {
        return await _context.RefreshTokens
            .FirstOrDefaultAsync(u => u.UserId == userId);
    }
    
    public async Task AddAsync(RefreshToken token)
    {
        await _context.RefreshTokens.AddAsync(token);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}