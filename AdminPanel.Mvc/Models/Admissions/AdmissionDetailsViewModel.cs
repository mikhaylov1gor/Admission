using AdmissionService.Application.Dtos.Responses;

namespace AdminPanel.Mvc.Models.Admissions;

public class AdmissionDetailsViewModel
{
    public StudentAdmissionDto StudentAdmission { get; set; }
    public Guid? ApplicantId { get; set; }
    
    public bool IsMine { get; set; }
}