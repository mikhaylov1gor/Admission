using DocumentService.Domain.Entities;
using DocumentService.Domain.IRepositories;
using DocumentService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DocumentService.Infrastructure.Repositories;

public class EducationDocumentTypeRepository : IEducationDocumentTypeRepository
{
    private readonly DocumentDbContext _context;

    public EducationDocumentTypeRepository(DocumentDbContext context)
    {
        _context = context;
    }
    
    public async Task<EducationDocumentType?> GetEducationDocumentTypeByIdAsync(Guid id)
    {
        return await _context.EducationDocumentTypes.FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task AddAsync(EducationDocumentType entity)
    {
        await _context.EducationDocumentTypes.AddAsync(entity);
        await _context.SaveChangesAsync();
    }
}