using AdmissionService.Domain.Entities;
using Contract.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace AdmissionService.Infrastructure.Persistence;

public class AdmissionDbContext : DbContext
{
    public DbSet<StudentAdmission> StudentAdmissions { get; set; }
    public DbSet<AdmissionProgram> AdmissionPrograms { get; set; }
    public DbSet<AdmissionSettings> AdmissionSettings { get; set; }
    public AdmissionDbContext(DbContextOptions options) : base(options) {}
    
    public async Task EnsureSettingsCreated()
    {
        if (!await AdmissionSettings.AnyAsync())
        {
            var defaultSettings = new AdmissionSettings
            {
                CreatedTime = DateTime.UtcNow,
                AdmissionSeasonId = 1,
                UniversityAdmissionStatus = UniversityAdmissionStatus.Open
            };

            await AdmissionSettings.AddAsync(defaultSettings);
            await SaveChangesAsync();
        }
    }
}