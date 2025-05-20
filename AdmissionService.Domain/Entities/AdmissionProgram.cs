namespace AdmissionService.Domain.Entities;

public class AdmissionProgram
{
    public Guid EducationProgramId { get; set; }
    public Guid StudentAdmissionId { get; set; }
    
    public int Priority { get; set; }
    public required StudentAdmission StudentAdmission { get; set; }
}