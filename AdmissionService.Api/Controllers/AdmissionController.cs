using AdmissionService.Application.Dtos.Requests;
using AdmissionService.Application.Services.AdmissionService;
using AdmissionService.Application.Services.DictionaryServiceClient;
using AdmissionService.Application.Services.ProgramService;
using Contract.Api.Controller;
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

    [HttpPost("Admission/Create")]
    public async Task<IActionResult> CreateAdmission()
    {
        var response = await _admissionService.CreateAdmission(UserId);
        return Ok(response);
    }

    [HttpGet("Admission/{admissionId}")]
    public async Task<IActionResult> GetAdmissionById(Guid admissionId)
    {
        var response = await _admissionService.GetAdmissionById(admissionId, UserId);
        return Ok(response);
    }

    [HttpPost("Admission/Program/Add")]
    public async Task<IActionResult> AddProgram(AddProgramDto dto)
    {
        await _programService.AddProgramToAdmission(dto, UserId);
        return Ok();
    }

    [HttpPut("Admission/Programs/Edit")]
    public async Task<IActionResult> EditPrograms(EditProgramsDto dto)
    {
        await _programService.EditAdmissionPrograms(dto, UserId);
        return Ok();
    }

    [HttpDelete("Admission/Program/{programId}/Remove")]
    public async Task<IActionResult> RemoveProgram(Guid programId, Guid userId)
    {
        await _programService.RemoveProgramFromAdmission(programId, userId);
        return Ok();
    }
}