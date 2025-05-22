using AdmissionService.Application.Dtos.Requests;

namespace AdmissionService.Application.Services.ProgramService;

public interface IProgramService
{
    Task AddProgramToAdmission(AddProgramDto dto, Guid userId);
    Task EditAdmissionPrograms(EditProgramsDto dto, Guid userId);
    Task RemoveProgramFromAdmission(Guid programId, Guid userId);
}