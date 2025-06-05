using UserService.Domain.Entities;

namespace UserService.Domain.IRepositories;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id);
    Task<User?> GetByEmailAsync(string email);
    Task AddAsync(User user);
    Task SaveChangesAsync();
    
    Task<List<Applicant>> GetAllApplicants();
    Task<List<Manager>> GetAllManagers(bool isAllManagers);
}