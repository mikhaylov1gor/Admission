using System.Security.Claims;
using Contract.Domain.Enums;
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

    [HttpGet("User/{userId}/Email")]
    [Authorize(Roles = "Administrator, Manager, SeniorManager")]
    public async Task<IActionResult> getUserEmail(Guid userId)
    {
        var response = await _applicantService.GetEmailByUserId(userId);
        return Ok(response);
    }

    [HttpGet("Applicants")]
    [Authorize(Roles = "Administrator, Manager, SeniorManager")]
    public async Task<IActionResult> GetApplicants()
    {
        var response = await _applicantService.GetAllApplicants();
        return Ok(response);
    }

    [HttpGet("Managers")]
    [Authorize(Roles = "Administrator, SeniorManager")]
    public async Task<IActionResult> GetManagers()
    {
        var response = await _applicantService.GetAllManagers();
        return Ok(response);
    }

    [HttpPost("Users/{userId}/AssignRole")]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> AssignRoleTo(Guid userId, Role role)
    {
        await _applicantService.AssignRoleTo(userId, role);
        return Ok();
    }

    [HttpGet("Applicant/{applicantId}/Profile")]
    [Authorize(Roles = "Administrator, Manager, SeniorManager")]
    public async Task<IActionResult> GetApplicantProfile(Guid applicantId)
    {
        var response = await _applicantService.GetProfile(applicantId);
        return Ok(response.Value);
    }
}