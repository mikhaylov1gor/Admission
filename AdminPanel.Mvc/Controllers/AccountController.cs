using System.Security.Claims;
using AdminPanel.Mvc.Models.Account;
using AdminPanel.Mvc.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using AdminPanel.Mvc.Services.AuthService;
using Microsoft.AspNetCore.Authorization;

namespace AdminPanel.Mvc.Controllers;

public class AccountController : Controller
{
    private readonly IAuthService _authService;

    public AccountController(
        IAuthService authService)
    {
        _authService = authService;
    }

    [HttpGet]
    public IActionResult Login() => View();

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel loginViewModel)
    {
        if (!ModelState.IsValid)
        {
            return View(loginViewModel);
        }

        var identity = await _authService.Login(loginViewModel);
        
        if (identity == null)
        {
            ModelState.AddModelError("", "Неверный email или пароль");
            return View(loginViewModel);
        }

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(identity),
            new AuthenticationProperties
            {
                IsPersistent = loginViewModel.RememberMe 
            });

        return RedirectToAction("Index", "Home");
    }

    [Authorize]
    public async Task<IActionResult> Logout()
    {
        Response.Cookies.Delete("AccessToken");
        Response.Cookies.Delete("RefreshToken");
    
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Login");
    }
}