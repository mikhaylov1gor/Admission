using AdminPanel.Mvc.Models.Applicant;
using AdminPanel.Mvc.Services.UserServiceClient;
using AdmissionService.Application.Services.DocumentServiceClient;
using DocumentService.Application.Dtos.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using IDocumentServiceClient = AdminPanel.Mvc.Services.DocumentServiceClient.IDocumentServiceClient;

namespace AdminPanel.Mvc.Controllers;

[Authorize(Roles = "Administrator, Manager, SeniorManager")]
public class ApplicantController : Controller
{
    private readonly IUserServiceClient _userServiceClient;
    private readonly IDocumentServiceClient _documentServiceClient;
    private readonly ILogger<ApplicantController> _logger;

    public ApplicantController(
        IUserServiceClient userServiceClient,
        IDocumentServiceClient documentServiceClient,
        ILogger<ApplicantController> logger)
    {
        _userServiceClient = userServiceClient;
        _documentServiceClient = documentServiceClient;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        try
        {
            var applicants = await _userServiceClient.GetAllApplicants();

            var viewModel = new ApplicantViewModel
            {
                Applicants = applicants.ToList()
            };

            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting applicants list");
            TempData["Error"] = "Ошибка при получении списка абитуриентов";
            return View(new ApplicantViewModel());
        }
    }
    
    [HttpPost("Applicant/AssignRole/{userId}")]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> AssignRole(Guid userId, string role)
    {
        try
        {
            if (!new[] { "Manager", "SeniorManager", "Applicant" }.Contains(role))
            {
                TempData["Error"] = "Некорректная роль";
                return RedirectToAction(nameof(Index));
            }

            var result = await _userServiceClient.AssignRole(userId, role);
            if (result)
            {
                TempData["Success"] = "Роль успешно изменена";
            }
            else
            {
                TempData["Error"] = "Не удалось изменить роль";
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error assigning role");
            TempData["Error"] = "Ошибка при изменении роли";
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Details(Guid applicantId)
    {
        try
        {
            var applicant = await _userServiceClient.GetApplicant(applicantId);

            
            var passportTask = await _documentServiceClient.GetPassport(applicantId);
            var educationDocumentTask = await _documentServiceClient.GetEducationDocument(applicantId);
            
            
            var viewModel = new ApplicantDetailsViewModel
            {
                Applicant = applicant,
                Passport = passportTask != null ? passportTask : null,
                EducationDocument = educationDocumentTask != null ? educationDocumentTask : null,
            };

            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при получении деталей абитуриента {ApplicantId}", applicantId);
            TempData["Error"] = "Произошла ошибка при загрузке данных";
            return RedirectToAction("Index");
            
        }
    }
    
    [HttpGet("DownloadScan/{applicantId}/{scanId}")]
    public async Task<IActionResult> DownloadScan(Guid scanId, Guid applicantId)
    {
        try
        {
            var file = await _documentServiceClient.DownloadScan(scanId, applicantId);
            return File(file.Data, file.ContentType, file.FileName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error downloading scan");
            return StatusCode(500, "Error downloading file");
        }
    }
}