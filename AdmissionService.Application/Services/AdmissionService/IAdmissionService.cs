using AdmissionService.Application.Dtos.Responses;

namespace AdmissionService.Application.Services.AdmissionService;

public interface IAdmissionService
{
    Task <Guid> CreateAdmission(Guid userId);
    
    Task <StudentAdmissionDto> GetAdmissionById(Guid admissionId, Guid userId);
    Task<List<ShortAdmissionDto>> GetMyAdmissions(Guid userId);
    Task<bool> IsAdmissionOpen();
}