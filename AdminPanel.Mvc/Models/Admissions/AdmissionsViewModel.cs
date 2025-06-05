using Contract.Dtos.Dtos.Requests;
using Contract.Dtos.Dtos.Responses;
using UserService.Application.Dtos.Responses;

namespace AdminPanel.Mvc.Models.Admissions;

public class AdmissionsViewModel
{
    public AdmissionsDto Admissions { get; set; } = new();
    public GetAdmissionsDto Filters { get; set; } = new(); 
    public List<ManagerDto> Managers { get; set; } = new();
}