using DocumentService.Domain.Entities;
using File = DocumentService.Domain.Entities.File;

namespace DocumentService.Domain.IRepositories;

public interface IDocumentRepository
{
    Task<Document?> GetDocumentByUserIdAndDocumentIdAsync(Guid documentId, Guid userId);
    Task<File?> GetFileByScanIdAndUserIdAsync(Guid scanId, Guid userId);
    Task AddFileAsync(File file);
    Task DeleteFileAsync(File file);
    Task SaveChangesAsync();
}