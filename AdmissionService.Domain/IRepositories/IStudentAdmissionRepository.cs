using AdmissionService.Domain.Entities;

namespace AdmissionService.Domain.IRepositories;

public interface IStudentAdmissionRepository
{
    Task<bool> IsStudentAdmissionExists(Guid applicantId);
    Task AddStudentAdmissionAsync(StudentAdmission studentAdmission);
    Task<StudentAdmission?> GetCurrentStudentAdmissionAsync(Guid applicantId);
    Task<StudentAdmission?> GetStudentAdmissionByIdAsync(Guid studentAdmissionId, Guid userId);
    Task DeleteStudentAdmissionProgramByIdAsync(Guid studentAdmissionId, Guid programId);
    Task UpdateAdmissionProgram(AdmissionProgram program);
    
    Task<List<StudentAdmission>?> GetStudentAdmissions(Guid userId);
    
    Task AddAdmissionProgramAsync(AdmissionProgram program);
    Task SaveChangesAsync();
    
}