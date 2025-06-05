using Contract.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using UserService.Domain.Entities;
using UserService.Domain.IRepositories;
using UserService.Infrastructure.Persistence;

namespace UserService.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly UserDbContext _context;

    public UserRepository(UserDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByIdAsync(Guid id)
    {
        return await _context.Users
            .FirstOrDefaultAsync(user => user.Id == id);
    }

    public async Task<List<Applicant>> GetAllApplicants()
    {
        return await _context.Applicants
            .Include(a => a.User)
            .ToListAsync();
    }

    public async Task<List<Manager>> GetAllManagers(bool isAllManagers)
    {
        var query = _context.Managers
            .Include(manager => manager.User)
            .AsQueryable();

        if (!isAllManagers)
        {
            query = query.Where(manager => manager.User != null && 
                                           manager.User.Role == Role.Manager);
        }

        return await query.ToListAsync();
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _context.Users
            .FirstOrDefaultAsync(user => user.Email == email);
    }

    public async Task AddAsync(User user)
    {
        await _context.Users.AddAsync(user);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
    
    
}