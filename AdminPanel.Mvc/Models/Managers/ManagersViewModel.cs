using UserService.Application.Dtos.Responses;

namespace AdminPanel.Mvc.Models.Managers;


public class ManagersViewModel
{
    public List<ManagerDto> RegularManagers { get; set; } = new();
    public List<ManagerDto> SeniorManagers { get; set; } = new();
    
}
