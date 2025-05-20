using DocumentService.Domain.Entities;
using DocumentService.Domain.IRepositories;
using DocumentService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using File = DocumentService.Domain.Entities.File;

namespace DocumentService.Infrastructure.Repositories;

public class DocumentRepository : IDocumentRepository
{
    private readonly DocumentDbContext _context;

    public DocumentRepository(DocumentDbContext context)
    {
        _context = context;
    }

    public async Task<Document?> GetDocumentByUserIdAndDocumentIdAsync(Guid documentId, Guid userId)
    {
        return await _context.Documents
            .FirstOrDefaultAsync(d => d.Id == documentId && d.ApplicantId == userId);
    }

    public async Task<File?> GetFileByScanIdAndUserIdAsync(Guid scanId, Guid userId)
    {
        return await _context.Files
            .FirstOrDefaultAsync(f => f.Id == scanId && f.Document.ApplicantId == userId);
    }
    
    public async Task AddFileAsync(File file)
    {
        await _context.Files.AddAsync(file);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteFileAsync(File file)
    {
        _context.Files.Remove(file);
        await _context.SaveChangesAsync();
    }
    
    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}