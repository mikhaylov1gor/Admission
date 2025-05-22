namespace AdmissionService.Application.Dtos.Responses;

public class AdmissionProgramDto
{
    public Guid Id { get; init; }
    public DateTime CreatedTime { get; init; }
    public DateTime? ModifiedTime { get; init; }
    public int Priority { get; init; }
    public EducationProgramDto EducationProgram { get; init; } = null!;
}