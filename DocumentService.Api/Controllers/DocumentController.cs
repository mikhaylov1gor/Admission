using Contract.Api.Controller;
using DocumentService.Application.Dtos.Requests;
using DocumentService.Application.Dtos.Responses;
using DocumentService.Application.Services.DocumentService;
using Microsoft.AspNetCore.Mvc;

namespace DocumentService.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DocumentController : BaseController
{
    private readonly IDocumentService _documentService;

    public DocumentController(IDocumentService documentService)
    {
        _documentService = documentService;
    }

    [HttpPost("{id}/Scan")]
    public async Task<IActionResult> UploadFileToDocument(Guid id, [FromForm] string scan)
    {
        var response = await _documentService.UploadScanToDocument(id, scan, UserId);
        return Ok(response);
    }

    [HttpGet("Scan/{id}")]
    public async Task<IActionResult> DownloadScan(Guid id)
    {
        var response = await _documentService.DownloadDocumentScan(id, UserId);
        return Ok(response.Value);
    }

    [HttpPut("Scan/{id}")]
    public async Task<IActionResult> EditScan(Guid id, [FromForm] string scan)
    {
        var response = await _documentService.UpdateDocumentScan(id, scan, UserId);
        return Ok(response);
    }

    [HttpDelete("Scan/{id}")]
    public async Task<IActionResult> DeleteScan(Guid id)
    {
        var response = await _documentService.DeleteDocumentScan(id, UserId);
        return Ok(response);
    }

    [HttpPost("Passport")]
    public async Task<IActionResult> CreatePassport(CreatePassportDto dto)
    {
        var response = await _documentService.CreatePassport(dto, UserId);
        return Ok(response);
    }

    [HttpGet("Passport")]
    public async Task<IActionResult> GetPassport()
    {
        var response = await _documentService.GetPassport(UserId);
        return Ok(response.Value);
    }

    [HttpPut("Passport")]
    public async Task<IActionResult> EditPassport(EditPassportDto dto)
    {
        var response = await _documentService.EditPassport(dto, UserId);
        return Ok(response);
    }

    [HttpDelete("Passport")]
    public async Task<IActionResult> DeletePassport()
    {
        var response = await _documentService.DeletePassport(UserId);
        return Ok(response);
    }

    [HttpPost("Education")]
    public async Task<IActionResult> CreateEducationDocument(CreateEducationDocumentDto dto)
    {
        var response = await _documentService.CreateEducationDocument(dto, UserId);
        return Ok(response);
    }

    [HttpGet("Education")]
    public async Task<IActionResult> GetEducationDocument() 
    {
        var response = await _documentService.GetEducationDocument(UserId);
        return Ok(response.Value);
    }

    [HttpPut("Education")]
    public async Task<IActionResult> EditEducationDocument(EditEducationDocumentDto dto)
    {
        var response = await _documentService.EditEducationDocument(dto, UserId);
        return Ok(response);
    }

    [HttpDelete("Education")]
    public async Task<IActionResult> DeleteEducationDocument()
    {
        var response = await _documentService.DeleteEducationDocument(UserId);
        return Ok(response);
    }
}
    
    