namespace AdmissionService.Domain.Entities;

public class AdmissionProgram
{
    public Guid Id { get; set; }
    public Guid StudentAdmissionId { get; set; }
    public required DateTime CreatedTime {get;set;}
    public DateTime? ModifiedTime {get;set;}
    
    public int Priority { get; set; }
    public required StudentAdmission StudentAdmission { get; set; }
}