using Contract.Dtos.Dtos.Requests;
using Contract.Dtos.Dtos.Responses;

namespace AdminPanel.Mvc.Models.Admissions;

public class AdmissionsViewModel
{
    public AdmissionsDto Admissions { get; set; } = new();
    public GetAdmissionsDto Filters { get; set; } = new(); 
}