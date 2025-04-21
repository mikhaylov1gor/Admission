using Microsoft.EntityFrameworkCore;
using UserService.Domain.Entities;
using UserService.Domain.IRepositories;
using UserService.Infrastructure.Persistence;

namespace UserService.Infrastructure.Repositories;

public class ApplicantRepository : IApplicantRepository
{
    private readonly UserDbContext _context;

    public ApplicantRepository(UserDbContext context)
    {
        _context = context;
    }

    public async Task<Applicant?> GetByIdAsync(Guid id)
    {
        return await _context.Applicants
            .Include(applicant => applicant.User) 
            .FirstOrDefaultAsync(applicant => applicant.Id == id);
    }

    public async Task<Applicant?> GetByUserIdAsync(Guid userId)
    {
        return await _context.Applicants
            .Include(applicant => applicant.User)
            .FirstOrDefaultAsync(user => user.Id == userId);
    }

    public async Task<List<Applicant>> GetAllAsync()
    {
        return await _context.Applicants
            .Include(applicant => applicant.User)
            .ToListAsync();
    }

    public async Task AddAsync(Applicant applicant)
    {
        await _context.Applicants.AddAsync(applicant);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}