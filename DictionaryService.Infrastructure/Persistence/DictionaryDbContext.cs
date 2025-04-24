using DictionaryService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DictionaryService.Infrastructure.Persistence;

public class DictionaryDbContext : DbContext
{
    public DbSet<Faculty> Faculties { get; set; }
    public DbSet<EducationProgram> EducationPrograms { get; set; }
    public DbSet<EducationLevel> EducationLevels { get; set; }
    public DbSet<EducationDocumentType> EducationDocumentTypes { get; set; }
    
    public DictionaryDbContext(DbContextOptions options) : base(options) { }
}