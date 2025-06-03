using AdmissionService.Application.Dtos.Responses;
using Contract.Domain.Enums;
using Contract.Dtos.Dtos.Requests;
using Contract.Dtos.Dtos.Responses;
using StudentAdmissionDto = AdmissionService.Application.Dtos.Responses.StudentAdmissionDto;

namespace AdmissionService.Application.Services.AdmissionService;

public interface IAdmissionService
{
    Task <Guid> CreateAdmission(Guid userId);
    
    Task <StudentAdmissionDto> GetAdmissionById(Guid admissionId, Guid userId, bool isWorker);
    Task<List<ShortAdmissionDto>> GetMyAdmissions(Guid userId);
    Task<bool> IsAdmissionOpen();
    
    Task<bool> IsMine(Guid admissionId, Guid userId);
    
    Task ChangeAdmissionStatus(Guid admissionId, AdmissionStatus newStatus, Guid managerId, bool isGigaWorker);
    Task<AdmissionsDto> GetAdmissions(GetAdmissionsDto dto, Guid workerId);
    
    Task TakeUntakeAdmission(Guid admissionId, Guid workerId, Guid userId);
}