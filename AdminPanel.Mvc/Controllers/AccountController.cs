using AdminPanel.Mvc.Models;
using AdminPanel.Mvc.Models.Account;
using AdminPanel.Mvc.Services.AuthService;
using Microsoft.AspNetCore.Mvc;

namespace AdminPanel.Mvc.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAuthServiceClient _authServiceClient;

        public AccountController(
            IAuthServiceClient authServiceClient)
        {
            _authServiceClient = authServiceClient;
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await _authServiceClient.LoginAsync(model.Email, model.Password);

            if (result.Success)
            {
                HttpContext.Session.SetString("JWT", result.AccessToken);
                return RedirectToAction("Index", "Home");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }

            return View(model);
        }
    }
}