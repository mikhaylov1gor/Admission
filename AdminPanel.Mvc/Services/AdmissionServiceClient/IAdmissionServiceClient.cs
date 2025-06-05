using AdmissionService.Application.Dtos.Requests;
using AdmissionService.Application.Dtos.Responses;
using Contract.Domain.Enums;
using Contract.Dtos.Dtos.Requests;
using Contract.Dtos.Dtos.Responses;
using DocumentService.Application.Dtos.Requests;

namespace AdminPanel.Mvc.Services.AdmissionServiceClient;

public interface IAdmissionServiceClient
{
    Task<AdmissionsDto?> GetAdmissions(GetAdmissionsDto admissionsDto);
    Task<bool> TakeUntakeAdmission(Guid admissionId, Guid applicantId);
    Task<StudentAdmissionDto> GetStudentAdmission(Guid admissionId);
    Task<bool> IsMineAdmission(Guid admissionId);
    Task <bool> ChangeStatusAsync(Guid admissionId, AdmissionStatus newStatus);
    Task<bool> RemoveProgramFromAdmissionAsync(Guid admissionId, Guid programId);
    Task<bool> ChangePriorities(Guid admissionId, EditProgramsDto dto);
    Task<bool> AssignManager(Guid admissionId, Guid managerId);
    Task<bool> IsMine(Guid applicantId);
}