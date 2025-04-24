using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserService.Api.Context.UserContext;
using UserService.Application.Dtos.Requests;
using UserService.Application.Services.ApplicantService;
using UserService.Application.Services.AuthService;

namespace UserService.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ApplicantController : ControllerBase
{
    private readonly IApplicantService _applicantService;
    private readonly IAuthService _authService;
    private readonly IUserContextService _userContextService;

    public ApplicantController(
        IApplicantService applicantService,
        IAuthService authService,
        IUserContextService userContextService)
    {
        _applicantService = applicantService;
        _authService = authService;
        _userContextService = userContextService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterUserDto dto)
    {
        var response = await _authService.RegisterApplicant(dto);
        return Ok(response.Value);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginCredentialsDto dto)
    {
        var response = await _authService.Login(dto);
        return Ok(response.Value);
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        var userId = _userContextService.GetCurrentUserId(); 
        await _authService.Logout(userId);
        return Ok();
    }

    [HttpPut("resetPassword")]
    [Authorize]
    public async Task<IActionResult> ResetPassword(ResetPasswordDto dto)
    {
        var userId = _userContextService.GetCurrentUserId(); 
        await _applicantService.ResetPassword(dto, userId);
        return Ok();
    }

    [HttpPost("refreshToken")]
    public async Task<IActionResult> RefreshToken(string token)
    {
        var response = await _authService.RefreshTokens(token);
        return Ok(response.Value);
    }

    [HttpGet("profile")]
    [Authorize]
    public async Task<IActionResult> GetProfile()
    {
        var userId = _userContextService.GetCurrentUserId(); 
        var response = await _applicantService.GetProfile(userId);
        return Ok(response.Value);
    }

    [HttpPut("profile")]
    [Authorize]
    public async Task<IActionResult> EditProfile(EditUserDto dto)
    {
        var userId = _userContextService.GetCurrentUserId(); 
        await _applicantService.EditProfile(dto, userId);
        return Ok();
    }
}