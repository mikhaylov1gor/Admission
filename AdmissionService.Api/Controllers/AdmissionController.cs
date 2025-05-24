using AdmissionService.Application.Dtos.Requests;
using AdmissionService.Application.Services.AdmissionService;
using AdmissionService.Application.Services.DictionaryServiceClient;
using AdmissionService.Application.Services.ProgramService;
using Contract.Api.Controller;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdmissionService.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AdmissionController : BaseController
{
    private readonly IAdmissionService _admissionService;
    private readonly IDictionaryServiceClient _dictionaryClient;
    private readonly IProgramService _programService;

    public AdmissionController(
        IAdmissionService admissionService,
        IDictionaryServiceClient dictionaryClient,
        IProgramService programService)
    {
        _admissionService = admissionService;
        _dictionaryClient = dictionaryClient;
        _programService = programService;
    }

    [Authorize(Roles = "Applicant")]
    [HttpPost("Admission/Create")]
    public async Task<IActionResult> CreateAdmission()
    {
        var response = await _admissionService.CreateAdmission(UserId);
        return Ok(response);
    }

    [Authorize(Roles = "Applicant")]
    [HttpGet("Admissions/My")]
    public async Task<IActionResult> GetMyAdmissions()
    {
        var response = await _admissionService.GetMyAdmissions(UserId);
        return Ok(response);
    }

    [Authorize(Roles = "Applicant")]
    [HttpGet("Admission/{admissionId}")]
    public async Task<IActionResult> GetAdmissionById(Guid admissionId)
    {
        var response = await _admissionService.GetAdmissionById(admissionId, UserId);
        return Ok(response);
    }

    [Authorize(Roles = "Applicant")]
    [HttpPost("Admission/Program/Add")]
    public async Task<IActionResult> AddProgram(AddProgramDto dto)
    {
        await _programService.AddProgramToAdmission(dto, UserId);
        return Ok();
    }

    [Authorize(Roles = "Applicant")]
    [HttpPut("Admission/Programs/EditPriorities")]
    public async Task<IActionResult> EditPrograms(EditProgramsDto dto)
    {
        await _programService.EditAdmissionPrograms(dto, UserId);
        return Ok();
    }

    [Authorize(Roles = "Applicant")]
    [HttpDelete("Admission/Program/{programId}/Remove")]
    public async Task<IActionResult> RemoveProgram(Guid programId)
    {
        await _programService.RemoveProgramFromAdmission(programId, UserId);
        return Ok();
    }

    [HttpGet("University/IsOpen")]
    public async Task<IActionResult> GetUniversityAdmissionStatus()
    {
        var response = await _admissionService.IsAdmissionOpen();
        return Ok(response);
    }
}