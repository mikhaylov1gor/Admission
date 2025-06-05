using System.Security.Claims;
using AdmissionService.Application.Dtos.Requests;
using AdmissionService.Application.Services.AdmissionService;
using AdmissionService.Application.Services.ProgramService;
using Contract.Api.Controller;
using Contract.Domain.Enums;
using Contract.Dtos.Dtos.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdmissionService.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AdminController : BaseController
{
    private readonly IAdmissionService _admissionService;
    private readonly IProgramService _programService;

    public AdminController(IAdmissionService admissionService,
        IProgramService programService)
    {
        _admissionService = admissionService;
        _programService = programService;
    }

    [HttpGet("Admissions/Get")]
    [Authorize(Roles = "Administrator, Manager, SeniorManager")]
    public async Task<IActionResult> GetAdmissions([FromQuery]GetAdmissionsDto dto)
    {
        var response = await _admissionService.GetAdmissions(dto, UserId);
        return Ok(response);
    }

    [HttpPatch("Admission/{admissionId}/TakeUntake")]
    [Authorize(Roles = "Administrator, Manager, SeniorManager")]
    public async Task<IActionResult> TakeUntakeAdmission(Guid admissionId, Guid applicantId)
    {
        await _admissionService.TakeUntakeAdmission(admissionId, UserId, applicantId);
        return Ok();
    }

    [HttpGet("Admission/{admissionId}/IsMine")]
    [Authorize(Roles = "Administrator, Manager, SeniorManager")]
    public async Task<IActionResult> IsMineAdmission(Guid admissionId)
    {
        var role = HttpContext.User.FindFirst(ClaimTypes.Role)?.Value;
    
        if (role == "Administrator" || role == "SeniorManager")
            return Ok(true);
        
        var response = await _admissionService.IsMine(admissionId, UserId);
        return Ok(response);
    }

    [HttpPut("Admission/{admissionId}/ChangeStatus")]
    [Authorize(Roles = "Administrator, Manager, SeniorManager")]
    public async Task<IActionResult> ChangeStatusAdmission(Guid admissionId, [FromQuery] AdmissionStatus status)
    {
        var role = HttpContext.User.FindFirst(ClaimTypes.Role)?.Value;
        
        var isGigaWorker = role == "Administrator" || role == "SeniorManager";
        
        await _admissionService.ChangeAdmissionStatus(admissionId, status, UserId, isGigaWorker);
        return Ok();
    }

    [HttpDelete("Admissions/{admissionId}/Programs/{programId}")]
    [Authorize(Roles = "Administrator, Manager, SeniorManager")]
    public async Task<IActionResult> RemoveProgramFromAdmission(Guid admissionId, Guid programId)
    {
        var role = HttpContext.User.FindFirst(ClaimTypes.Role)?.Value;
        
        var isGigaWorker = role == "Administrator" || role == "SeniorManager";
        
        await _programService.RemoveProgramForManager(admissionId, programId, UserId, isGigaWorker);
        return Ok();
    }

    [HttpPut("Admissions/{admissionId}/Priorities")]
    public async Task<IActionResult> ChangePriorities(Guid admissionId, EditProgramsDto dto)
    {
        var role = HttpContext.User.FindFirst(ClaimTypes.Role)?.Value;
        
        var isGigaWorker = role == "Administrator" || role == "SeniorManager";
        
        await _programService.EditAdmissionProgramsForManager(admissionId, dto, UserId, isGigaWorker);
        return Ok();
    }

    [HttpPut("Admissions/{admissionId}/AssignManager/{managerId}")]
    [Authorize(Roles = "Administrator, SeniorManager")]
    public async Task<IActionResult> AssignManagerToAdmission(Guid admissionId, Guid managerId)
    {
        await _admissionService.AssignManager(admissionId, managerId);
        return Ok();
    }

    [HttpGet("Applicants/{applicantId}")]
    [Authorize(Roles = "Manager, Administrator, SeniorManager")]
    public async Task<IActionResult> IsMineApplicant(Guid applicantId)
    {
        var role = HttpContext.User.FindFirst(ClaimTypes.Role)?.Value;
        
        var isGigaWorker = role == "Administrator" || role == "SeniorManager";
        
        var response = await _admissionService.IsMineApplicantByAdmission(applicantId, isGigaWorker, UserId);
        return Ok(response);
    }
}