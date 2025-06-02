using Microsoft.EntityFrameworkCore;
using UserService.Domain.Entities;
using UserService.Domain.IRepositories;
using UserService.Infrastructure.Persistence;

namespace UserService.Infrastructure.Repositories;

public class ManagerRepository : IManagerRepository
{
    private readonly UserDbContext _context;

    public ManagerRepository(UserDbContext context)
    {
        _context = context;
    }

    public async Task<Manager?> GetByIdAsync(Guid id)
    {
        return await _context.Managers
            .Include(manager => manager.User) 
            .FirstOrDefaultAsync(manager => manager.Id == id);
    }

    public async Task<List<Manager>> GetAllAsync()
    {
        return await _context.Managers
            .Include(manager => manager.User)
            .ToListAsync();
    }

    public async Task AddAsync(Manager manager)
    {
        await _context.Managers.AddAsync(manager);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid managerId)
    {
        var manager =await _context.Managers.FindAsync(managerId);
        if (manager != null)
        {
            _context.Remove(manager);
        }
    }
}