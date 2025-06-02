using AdminPanel.Mvc.Models.Managers;
using AdminPanel.Mvc.Services.AdmissionServiceClient;
using AdminPanel.Mvc.Services.UserServiceClient;
using Contract.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdminPanel.Mvc.Controllers;

[Authorize(Roles = "SeniorManager, Administrator")]
public class ManagerController : Controller
{
    private readonly IUserServiceClient  _userServiceClient;

    public ManagerController(IUserServiceClient userServiceClient)
    {
        _userServiceClient = userServiceClient;
    }
    
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        try
        {
            var managers = await _userServiceClient.GetAllManagers();
            
            var viewModel = new ManagersViewModel
            {
                RegularManagers = managers.Where(m => m.Role == Role.Manager).ToList(),
                SeniorManagers = managers.Where(m => m.Role == Role.SeniorManager).ToList()
            };

            return View(viewModel);
        }
        catch (Exception ex)
        {
            TempData["Error"] = "Ошибка при получении списка менеджеров";
            return View(new ManagersViewModel());
        }
    }
    
    [HttpPost("Manager/AssignRole/{userId}")]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> AssignRole(Guid userId, string role)
    {
        try
        {
            if (!new[] { "Manager", "SeniorManager", "Applicant" }.Contains(role))
            {
                TempData["Error"] = "Некорректная роль";
                return RedirectToAction(nameof(Index));
            }

            var result = await _userServiceClient.AssignRole(userId, role);
            if (result)
            {
                TempData["Success"] = "Роль успешно изменена";
            }
            else
            {
                TempData["Error"] = "Не удалось изменить роль";
            }
        }
        catch (Exception ex)
        {
            TempData["Error"] = "Ошибка при изменении роли";
        }

        return RedirectToAction(nameof(Index));
    }
    
}