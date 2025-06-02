using UserService.Domain.Entities;

namespace UserService.Domain.IRepositories;

public interface IApplicantRepository
{
    Task<Applicant?> GetByIdAsync(Guid id);
    Task<Applicant?> GetByUserIdAsync(Guid userId);
    Task<List<Applicant>> GetAllAsync();
    Task AddAsync(Applicant applicant);
    Task SaveChangesAsync();
    Task DeleteAsync(Guid applicantId);
}