using DictionaryService.Application.Dtos.Requests;
using DictionaryService.Application.Services.DictionaryService;
using Microsoft.AspNetCore.Mvc;

namespace DictionaryService.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DictionaryController : ControllerBase
{
    private readonly IDictionaryService _dictionaryService;
    public DictionaryController(IDictionaryService dictionaryService)
    {
        _dictionaryService = dictionaryService;
    }

    [HttpGet("educationLevels")]
    public async Task<IActionResult> GetEducationLevels()
    {
        var response = await _dictionaryService.GetEducationLevels();
        return Ok(response.Value);
    }

    [HttpGet("documentTypes")]
    public async Task<IActionResult> GetDocumentTypes()
    {
        var response = await _dictionaryService.GetDocumentTypes();
        return Ok(response.Value);
    }

    [HttpGet("faculties")]
    public async Task<IActionResult> GetFaculties()
    {
        var response = await _dictionaryService.GetFaculties();
        return Ok(response.Value);
    }

    [HttpGet("programs")]
    public async Task<IActionResult> GetPrograms([FromQuery] FilterParametersDto parameters)
    {
        var response = await _dictionaryService.GetPrograms(parameters);
        return Ok(response.Value);
    }
}