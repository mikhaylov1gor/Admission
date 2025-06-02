using UserService.Application.Dtos.Responses;

namespace AdminPanel.Mvc.Models.Applicant;

public class ApplicantViewModel
{
    public List<ApplicantDto> Applicants { get; set; } = new();
}