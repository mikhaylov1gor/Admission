using AdminPanel.Mvc.Models.Account;
using AdminPanel.Mvc.Services.AuthService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdminPanel.Mvc.Controllers;

[Authorize]
public class ProfileController : Controller
{
    private readonly IAuthService _authService;
    private readonly ILogger<ProfileController> _logger;

    public ProfileController(
        IAuthService authService,
        ILogger<ProfileController> logger)
    {
        _authService = authService;
        _logger = logger;
    }
    
    [HttpGet]
    public IActionResult ChangePassword()
    {
        return View(new ChangePasswordViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            var result = await _authService.ChangePassword(
                model.CurrentPassword,
                model.NewPassword);

            if (result.Success)
            {
                TempData["SuccessMessage"] = "Пароль успешно изменён";
                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError("", result.ErrorMessage ?? "Ошибка при смене пароля");
            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при смене пароля");
            ModelState.AddModelError("", "Произошла ошибка при смене пароля");
            return View(model);
        }
    }
}