using DocumentService.Application.Dtos.Requests;
using DocumentService.Application.Services.DocumentService;
using DocumentService.Application.Services.ScanService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DocumentService.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AdminController : ControllerBase
{
    private readonly IDocumentService _documentService;
    private readonly IScanService _scanService;
    public AdminController(IDocumentService documentService,
        IScanService scanService)
    {
        _documentService = documentService;
        _scanService = scanService;
    }

    [HttpGet("Applicant/{applicantId}/Passport")]
    [Authorize(Roles = "Manager, SeniorManager, Administrator")]
    public async Task<IActionResult> PassportInfo(Guid applicantId)
    {
        var response = await _documentService.GetPassport(applicantId);
        return Ok(response);
    }
    
    [HttpGet("Applicant/{applicantId}/EducationDocument")]
    [Authorize(Roles = "Manager, SeniorManager, Administrator")]
    public async Task<IActionResult> EducationDocumentInfo(Guid applicantId)
    {
        var response = await _documentService.GetEducationDocument(applicantId);
        return Ok(response);
    }

    [HttpGet("Applicant/{applicantId}/Scan/{scanId}")]
    [Authorize(Roles = "Manager, SeniorManager, Administrator")]
    public async Task<IActionResult> DownloadScan(Guid scanId, Guid applicantId)
    {
        var response = await _scanService.DownloadDocumentScan(scanId, applicantId);
        return File(response.Data, response.ContentType, response.FileName);
    }

    [HttpPut("Applicant/{applicantId}/ChangePassport")]
    [Authorize(Roles = "Manager, SeniorManager, Administrator")]
    public async Task<IActionResult> ChangePassport(EditPassportDto dto, Guid applicantId)
    {
        await _documentService.EditPassport(dto, applicantId);
        return Ok();
    }
    
    [HttpPut("Applicant/{applicantId}/ChangeEducationDocument")]
    [Authorize(Roles = "Manager, SeniorManager, Administrator")]
    public async Task<IActionResult> ChangePassport(EditEducationDocumentDto dto, Guid applicantId)
    {
        await _documentService.EditEducationDocument(dto, applicantId);
        return Ok();
    }

    [HttpDelete("Applicants/{applicantId}/Files/{scanId}")]
    [Authorize(Roles = "Manager, SeniorManager, Administrator")]
    public async Task<IActionResult> DeleteFile(Guid scanId, Guid applicantId)
    {
        await _scanService.DeleteDocumentScan(scanId, applicantId);
        return Ok();
    }

    [HttpPost("Applicants/{applicantId}/Documents/{documentId}")]
    [Authorize(Roles = "Manager, SeniorManager, Administrator")]
    public async Task<IActionResult> UploadScanToDocument(Guid applicantId, CreateScanDto dto, Guid documentId)
    {
        await _scanService.UploadScanToDocument(documentId, dto, applicantId);
        return Ok();
    }
}