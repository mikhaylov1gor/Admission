using AdmissionService.Application.Services.AdmissionService;
using Contract.Api.Controller;
using Contract.Dtos.Dtos.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdmissionService.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AdminController : BaseController
{
    private readonly IAdmissionService _admissionService;

    public AdminController(IAdmissionService admissionService)
    {
        _admissionService = admissionService;
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
}