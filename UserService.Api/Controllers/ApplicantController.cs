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
        var result = await _authService.RegisterApplicant(dto);
        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }
        return BadRequest(result.Error);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginCredentialsDto dto)
    {
        var result = await _authService.Login(dto);
        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }
        return BadRequest(result.Error);
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        var result = await _authService.Logout();
        if (result.IsSuccess)
        {
            return Ok();
        }
        return BadRequest(result.Error);
    }

    [HttpPut("resetPassword")]
    [Authorize]
    public async Task<IActionResult> ResetPassword(ResetPasswordDto dto)
    {
        var userId = _userContextService.GetCurrentUserId(); 
        var result = await _applicantService.ResetPassword(dto, userId);
        if (result.IsSuccess)
        {
            return Ok();
        }
        return BadRequest(result.Error);
    }

    [HttpPost("refreshToken")]
    [Authorize]
    public async Task<IActionResult> RefreshToken(string token)
    {
        var result = await _authService.RefreshTokens(token);
        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }
        return BadRequest(result.Error);
    }

    [HttpGet("profile")]
    [Authorize]
    public async Task<IActionResult> GetProfile()
    {
        var userId = _userContextService.GetCurrentUserId(); 
        var result = await _applicantService.GetProfile(userId);
        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }
        return BadRequest(result.Error);
    }

    [HttpPut("profile")]
    [Authorize]
    public async Task<IActionResult> EditProfile(EditUserDto dto)
    {
        var userId = _userContextService.GetCurrentUserId(); 
        var result = await _applicantService.EditProfile(dto, userId);
        if (result.IsSuccess)
        {
            return Ok(result);
        }
        return BadRequest(result.Error);
    }
}