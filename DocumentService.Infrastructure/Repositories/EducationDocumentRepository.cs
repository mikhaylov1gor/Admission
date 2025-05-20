using Contract.Application.Exceptions;
using DocumentService.Domain.Entities;
using DocumentService.Domain.IRepositories;
using DocumentService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DocumentService.Infrastructure.Repositories;

public class EducationDocumentRepository : IEducationDocumentRepository
{
    private readonly DocumentDbContext _context;

    public EducationDocumentRepository(DocumentDbContext context)
    {
        _context = context;
    }

    public async Task<EducationDocument?> GetByIdAsync(Guid id)
    {
        return await _context.Documents
            .OfType<EducationDocument>() 
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<EducationDocument?> GetByUserIdAsync(Guid userId)
    {
        return await _context.Documents
            .OfType<EducationDocument>()
            .Include(d => d.Files)
            .Include(ed => ed.EducationDocumentType)
            .Include(ed => ed.EducationDocumentType.EducationLevel)
            .FirstOrDefaultAsync(p => p.ApplicantId == userId);
    }

    public async Task<List<EducationDocument>> GetAllAsync()
    {
        return await _context.Documents
            .OfType<EducationDocument>()
            .ToListAsync();
    }

    public async Task DeleteEducationDocumentAsync(Guid userId)
    {
        var educationDocument = await _context.Documents
            .OfType<EducationDocument>()
            .FirstOrDefaultAsync(p => p.ApplicantId == userId);
        
        if (educationDocument == null)
        {
            throw new NotFoundException($"education document for user with id {userId} not found");
        }
        
        _context.Documents.Remove(educationDocument);
        await _context.SaveChangesAsync(); 
    }
    public async Task AddAsync(EducationDocument educationDocument)
    {
        await _context.EducationDocuments.AddAsync(educationDocument);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}