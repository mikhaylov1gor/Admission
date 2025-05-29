using Contract.Domain.Enums;

namespace Contract.Dtos.Dtos.Responses;

public class AdmissionDto
{
    public required Guid AdmissionId { get; set; }
    public required AdmissionStatus Status { get; set; }
    
    public required Guid ApplicantId { get; set; }
    
    public required bool IsManagerExists { get; set; }
    public required Guid? ManagerId { get; set; }
    
    public required bool IsMine { get; init; }
    public bool IsEditable { get; set; }

}
