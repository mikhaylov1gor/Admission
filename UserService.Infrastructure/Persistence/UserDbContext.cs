using Contract.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UserService.Domain.Entities;
using UserService.Infrastructure.Security;

namespace UserService.Infrastructure.Persistence;

public class UserDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Manager> Managers { get; set; }
    public DbSet<Applicant> Applicants { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    
    public UserDbContext(DbContextOptions options) : base(options) {}
    
    public async Task EnsureAdminCreated()
    {
        if (!await Users.AnyAsync(u => u.Role == Role.Administrator))
        {
            var adminUser = new User
            {
                Id = Guid.NewGuid(),
                Role = Role.Administrator,
                Email = "admin1@admin.com",
                HashedPassword = new PasswordHasherService().HashPassword("admin1"),
            };

            await Users.AddAsync(adminUser);
            await SaveChangesAsync();
        }
    }
}