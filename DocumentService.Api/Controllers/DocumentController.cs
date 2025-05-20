using Contract.Api.Controller;
using DocumentService.Application.Dtos.Requests;
using DocumentService.Application.Dtos.Responses;
using DocumentService.Application.Services.DocumentService;
using DocumentService.Application.Services.ScanService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DocumentService.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DocumentController : BaseController
{
    private readonly IDocumentService _documentService;
    private readonly IScanService _scanService;

    public DocumentController(
        IDocumentService documentService,
        IScanService scanService)
    {
        _documentService = documentService;
        _scanService = scanService;
    }

    [Authorize(Roles = "Applicant")]
    [HttpPost("{documentId}/Scan")]
    public async Task<IActionResult> UploadFileToDocument(Guid documentId, CreateScanDto dto)
    {
        var response = await _scanService.UploadScanToDocument(documentId, dto, UserId);
        return Ok(response);
    }

    [Authorize(Roles = "Applicant")]
    [HttpGet("Scan/{scanId}")]
    public async Task<IActionResult> DownloadScan(Guid scanId)
    {
        var response = await _scanService.DownloadDocumentScan(scanId, UserId);
        return File(response.Data, response.ContentType, response.FileName);
    }

    [Authorize(Roles = "Applicant")]
    [HttpPut("Scan/{scanId}")]
    public async Task<IActionResult> EditScan(Guid scanId, UpdateScanDto dto)
    {
        var response = await _scanService.UpdateDocumentScan(scanId, dto, UserId);
        return Ok(response);
    }

    [Authorize(Roles = "Applicant")]
    [HttpDelete("Scan/{scanId}")]
    public async Task<IActionResult> DeleteScan(Guid scanId)
    {
        var response = await _scanService.DeleteDocumentScan(scanId, UserId);
        return Ok(response);
    }

    [Authorize(Roles = "Applicant")]
    [HttpPost("Passport")]
    public async Task<IActionResult> CreatePassport(CreatePassportDto dto)
    {
        var response = await _documentService.CreatePassport(dto, UserId);
        return Ok(response);
    }

    [Authorize(Roles = "Applicant")]
    [HttpGet("Passport")]
    public async Task<IActionResult> GetPassport()
    {
        var response = await _documentService.GetPassport(UserId);
        return Ok(response);
    }

    [Authorize(Roles = "Applicant")]
    [HttpPut("Passport")]
    public async Task<IActionResult> EditPassport(EditPassportDto dto)
    {
        var response = await _documentService.EditPassport(dto, UserId);
        return Ok(response);
    }

    [Authorize(Roles = "Applicant")]
    [HttpDelete("Passport")]
    public async Task<IActionResult> DeletePassport()
    {
        var response = await _documentService.DeletePassport(UserId);
        return Ok(response);
    }

    [Authorize(Roles = "Applicant")]
    [HttpPost("Education")]
    public async Task<IActionResult> CreateEducationDocument(CreateEducationDocumentDto dto)
    {
        var response = await _documentService.CreateEducationDocument(dto, UserId);
        return Ok(response);
    }

    [Authorize(Roles = "Applicant")]
    [HttpGet("Education")]
    public async Task<IActionResult> GetEducationDocument() 
    {
        var response = await _documentService.GetEducationDocument(UserId);
        return Ok(response);
    }

    [Authorize(Roles = "Applicant")]
    [HttpPut("Education")]
    public async Task<IActionResult> EditEducationDocument(EditEducationDocumentDto dto)
    {
        var response = await _documentService.EditEducationDocument(dto, UserId);
        return Ok(response);
    }

    [Authorize(Roles = "Applicant")]
    [HttpDelete("Education")]
    public async Task<IActionResult> DeleteEducationDocument()
    {
        var response = await _documentService.DeleteEducationDocument(UserId);
        return Ok(response);
    }
}
    
    