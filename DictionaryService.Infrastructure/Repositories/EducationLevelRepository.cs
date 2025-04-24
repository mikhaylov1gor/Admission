using DictionaryService.Domain.Entities;
using DictionaryService.Domain.IRepositories;
using DictionaryService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DictionaryService.Infrastructure.Repositories;

public class EducationLevelRepository : IEducationLevelRepository
{
    private readonly DictionaryDbContext _context;

    public EducationLevelRepository(DictionaryDbContext context)
    {
        _context = context;
    }

    public async Task<List<EducationLevel>> GetAllAsync()
    {
        return await _context.EducationLevels
            .ToListAsync();
    }
}