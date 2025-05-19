using Contract.Application.Exceptions;
using DocumentService.Domain.Entities;
using DocumentService.Domain.IRepositories;
using DocumentService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DocumentService.Infrastructure.Repositories;

public class PassportRepository : IPassportRepository
{
    private readonly DocumentDbContext _context;

    public PassportRepository(DocumentDbContext context)
    {
        _context = context;
    }

    public async Task<Passport?> GetByIdAsync(Guid id)
    {
        return await _context.Documents
            .OfType<Passport>() 
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<Passport?> GetByUserIdAsync(Guid userId)
    {
        return await _context.Documents
            .OfType<Passport>()
            .FirstOrDefaultAsync(p => p.ApplicantId == userId);
    }

    public async Task<List<Passport>> GetAllAsync()
    {
        return await _context.Documents
            .OfType<Passport>()
            .ToListAsync();
    }

    public async Task DeletePassportAsync(Guid userId)
    {
        var passport = await _context.Documents
            .OfType<Passport>()
            .FirstOrDefaultAsync(p => p.ApplicantId == userId);
        
        if (passport == null)
        {
            throw new NotFoundException($"Passport for user with id {userId} not found");
        }
        
        _context.Documents.Remove(passport);
        await _context.SaveChangesAsync(); 
    }
    public async Task AddAsync(Passport passport)
    {
        await _context.Passports.AddAsync(passport);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}