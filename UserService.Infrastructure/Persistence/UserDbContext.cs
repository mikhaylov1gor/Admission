using Microsoft.EntityFrameworkCore;
using UserService.Domain.Entities;

namespace UserService.Infrastructure.Persistence;

public class UserDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Manager> Managers { get; set; }
    public DbSet<Applicant> Applicants { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    
    public UserDbContext(DbContextOptions options) : base(options) {}
}