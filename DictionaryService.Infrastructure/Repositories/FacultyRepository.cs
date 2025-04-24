using DictionaryService.Domain.Entities;
using DictionaryService.Domain.IRepositories;
using DictionaryService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DictionaryService.Infrastructure.Repositories;

public class FacultyRepository : IFacultyRepository
{
    private readonly DictionaryDbContext _context;

    public FacultyRepository(DictionaryDbContext context)
    {
        _context = context;
    }

    public async Task<List<Faculty>> GetAllAsync()
    {
        return await _context.Faculties
            .ToListAsync();
    }
}