using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserService.Application.Services.ApplicantService;
using UserService.Domain.Entities;
using UserService.Domain.IRepositories;

namespace UserService.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AdminController : ControllerBase
{
    private readonly IApplicantService _applicantService;
    
    public AdminController(IApplicantService applicantService)
    {
        _applicantService = applicantService;
    }

    [HttpGet("User/{userId}")]
    [Authorize(Roles = "Administrator, Manager, SeniorManager")]
    public async Task<IActionResult> GetRoleById(Guid userId)
    {
        var response = await _applicantService.GetRoleByUserId(userId);
        return Ok(response);
    }
}