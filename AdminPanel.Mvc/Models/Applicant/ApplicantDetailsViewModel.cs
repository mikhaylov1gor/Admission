using DocumentService.Application.Dtos.Responses;
using UserService.Application.Dtos.Responses;

namespace AdminPanel.Mvc.Models.Applicant;

public class ApplicantDetailsViewModel
{
    public ApplicantDto? Applicant { get; set; }
    public PassportDto? Passport { get; set; }
    public EducationDocumentDto? EducationDocument { get; set; }
}
