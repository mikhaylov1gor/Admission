using DocumentService.Application.Dtos.Requests;
using DocumentService.Application.Dtos.Responses;
using DocumentService.Application.Services.DocumentService;
using Microsoft.AspNetCore.Mvc;

namespace DocumentService.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DocumentController : ControllerBase
{
    private readonly IDocumentService _documentService;

    public DocumentController(IDocumentService documentService)
    {
        _documentService = documentService;
    }

    [HttpPost("{id}/Scan")]
    public async Task<IActionResult> UploadFileToDocument(Guid id, [FromForm] string scan)
    {
        return Ok();
    }

    [HttpGet("Scan/{id}")]
    public async Task<IActionResult> DownloadScan(Guid id)
    {
        return Ok();
    }

    [HttpPut("Scan/{id}")]
    public async Task<IActionResult> EditScan(Guid id, [FromForm] string scan)
    {
        return Ok();
    }

    [HttpDelete("Scan/{id}")]
    public async Task<IActionResult> DeleteScan(Guid id)
    {
        return Ok();
    }

    [HttpPost("Passport")]
    public async Task<IActionResult> CreatePassport(CreatePassportDto dto)
    {
        return Ok();
    }

    [HttpGet("Passport")]
    public async Task<IActionResult> GetPassport()
    {
        return Ok();
    }

    [HttpPut("Passport")]
    public async Task<IActionResult> EditPassport(EditPassportDto dto)
    {
        return Ok();
    }

    [HttpDelete("Passport")]
    public async Task<IActionResult> DeletePassport()
    {
        return Ok();
    }

    [HttpPost("Education")]
    public async Task<IActionResult> CreateEducationDocument(CreateEducationDocumentDto dto)
    {
        return Ok();
    }

    [HttpGet("Education")]
    public async Task<IActionResult> GetEducationDocument()
    {
        return Ok();
    }

    [HttpPut("Education")]
    public async Task<IActionResult> EditEducationDocument(EditEducationDocumentDto dto)
    {
        return Ok();
    }

    [HttpDelete("Education")]
    public async Task<IActionResult> DeleteEducationDocument()
    {
        return Ok();
    }
}
    
    