using Contract.Domain.Enums;

namespace AdmissionService.Domain.Entities;

public class StudentAdmission
{
    public Guid Id { get; set; }
    public DateTime CreatedTime { get; set; }
    public DateTime? ModifiedTime { get; set; }
    
    public Guid? ManagerId { get; set; }
    public required Guid ApplicantId { get; set; }
    public AdmissionStatus Status { get; set; }
    public ICollection<AdmissionProgram>? AdmissionPrograms { get; set; } = new List<AdmissionProgram>();
}