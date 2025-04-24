using Microsoft.AspNetCore.Mvc;

namespace DictionaryService.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DictionaryController : ControllerBase
{
    public DictionaryController()
    {
        
    }

    [HttpGet("educationLevels")]
    public async Task<IActionResult> GetEducationLevels()
    {
        return Ok();
    }

    [HttpGet("documentTypes")]
    public async Task<IActionResult> GetDocumentTypes()
    {
        return Ok();
    }

    [HttpGet("faculties")]
    public async Task<IActionResult> GetFaculties()
    {
        return Ok();
    }

    [HttpGet("programs")]
    public async Task<IActionResult> GetPrograms()
    {
        return Ok();
    }
}