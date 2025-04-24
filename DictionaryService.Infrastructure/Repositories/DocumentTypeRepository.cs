using DictionaryService.Domain.Entities;
using DictionaryService.Domain.IRepositories;
using DictionaryService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DictionaryService.Infrastructure.Repositories;

public class DocumentTypeRepository : IDocumentTypeRepository
{
    private readonly DictionaryDbContext _context;

    public DocumentTypeRepository(DictionaryDbContext context)
    {
        _context = context;
    }

    public async Task<List<EducationDocumentType>> GetAllAsync()
    {
        return await _context.EducationDocumentTypes
            .Include(el => el.EducationLevel)
            .Include(nel => nel.NextEducationLevels)
            .ToListAsync();
    }
}