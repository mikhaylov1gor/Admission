using AdmissionService.Application.Dtos.Requests;

namespace AdmissionService.Application.Services.ProgramService;

public interface IProgramService
{
    Task AddProgramToAdmission(AddProgramDto dto, Guid userId);
    Task EditAdmissionPrograms(EditProgramsDto dto, Guid userId);
    Task EditAdmissionProgramsForManager(Guid admissionId, EditProgramsDto dto, Guid managerId, bool isGigaWorker);
    Task RemoveProgramFromAdmission(Guid programId, Guid userId);
    Task RemoveProgramForManager(Guid admissionId, Guid programId, Guid managerId, bool isGigaWorker);
    
}