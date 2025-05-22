using Contract.Domain.Enums;

namespace AdmissionService.Domain.Entities;

public class AdmissionSettings
{
    public Guid Id { get; set; }
    public int AdmissionSeasonId { get; set; }
    public UniversityAdmissionStatus UniversityAdmissionStatus { get; set; }
    
    public required DateTime CreatedTime { get; set; }
    public DateTime? ModifiedTime { get; set; }
}