using DictionaryService.Domain.Entities;
using DictionaryService.Domain.IRepositories;
using DictionaryService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DictionaryService.Infrastructure.Repositories;

public class EducationProgramRepository : IEducationProgramRepository
{
    private readonly DictionaryDbContext _context;

    public EducationProgramRepository(DictionaryDbContext context)
    {
        _context = context;
    }

    public async Task<List<EducationProgram>> GetProgramsByParameters(
        string? facultyName,
        string? educationLevelName,
        string? educationForm,
        string? language,
        string? name)
    {
        var query = _context.EducationPrograms.AsQueryable();
        
        if (facultyName != null)
            query = query.Where(p => p.Faculty.Name.Contains(facultyName));
        
        if  (educationLevelName != null)
            query = query.Where(p => p.EducationLevel.Name.Contains(educationLevelName));
        
        if (educationForm != null)
            query = query.Where(p => p.EducationForm.Contains(educationForm));
        
        if (language != null)
            query = query.Where(p => p.Language.Contains(language));
        
        if (name != null)
            query = query.Where(p => p.Name.Contains(name) || p.Code != null && p.Code.Contains(name));
        
        return await query
            .Include(p => p.Faculty)
            .Include(p => p.EducationLevel)
            .ToListAsync();
    }
}