using AdmissionService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AdmissionService.Infrastructure.Persistence;

public class AdmissionDbContext : DbContext
{
    public DbSet<StudentAdmission> StudentAdmissions { get; set; }
    public DbSet<AdmissionProgram> AdmissionPrograms { get; set; }
    
    public AdmissionDbContext(DbContextOptions options) : base(options) {}
}