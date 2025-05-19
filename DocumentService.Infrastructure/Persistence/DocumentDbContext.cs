using DocumentService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using File = DocumentService.Domain.Entities.File;

namespace DocumentService.Infrastructure.Persistence;

public class DocumentDbContext : DbContext
{
    public DbSet<Document> Documents { get; set; }
    public DbSet<EducationDocument> EducationDocuments { get; set; }
    public DbSet<EducationDocumentType> EducationDocumentTypes { get; set; }
    public DbSet<EducationLevel> EducationLevels { get; set; }
    public DbSet<File> Files { get; set; }
    public DbSet<Passport> Passports { get; set; }
    
    public DocumentDbContext(DbContextOptions options) : base(options) {}
}