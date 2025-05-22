using Contract.Domain.Enums;

namespace AdmissionService.Application.Dtos.Responses;

public class StudentAdmissionDto
{
    public Guid Id { get; set; }
    public required DateTime CreatedTime { get; set; }
    public DateTime? ModifiedTime { get; set; }
    
    public AdmissionStatus Status { get; set; }
    public bool IsManagerExist { get; set; }
    public ICollection<AdmissionProgramDto> AdmissionPrograms { get; init; } = new List<AdmissionProgramDto>();
}