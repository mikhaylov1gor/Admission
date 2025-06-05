using System.Text.Json;
using AdminPanel.Mvc.Models.Applicant;
using AdminPanel.Mvc.Services.AdmissionServiceClient;
using AdminPanel.Mvc.Services.DictionaryServiceClient;
using AdminPanel.Mvc.Services.UserServiceClient;
using AdmissionService.Application.Services.AdmissionService;
using AdmissionService.Application.Services.DocumentServiceClient;
using DocumentService.Application.Dtos.Requests;
using DocumentService.Application.Dtos.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserService.Application.Dtos.Requests;
using IDocumentServiceClient = AdminPanel.Mvc.Services.DocumentServiceClient.IDocumentServiceClient;

namespace AdminPanel.Mvc.Controllers;

[Authorize(Roles = "Administrator, Manager, SeniorManager")]
public class ApplicantController : Controller
{
    private readonly IUserServiceClient _userServiceClient;
    private readonly IDocumentServiceClient _documentServiceClient;
    private readonly IDictionaryServiceClient _dictionaryServiceClient;
    private readonly ILogger<ApplicantController> _logger;
    private readonly IAdmissionServiceClient _admissionServiceClient;

    public ApplicantController(
        IUserServiceClient userServiceClient,
        IDocumentServiceClient documentServiceClient,
        ILogger<ApplicantController> logger,
        IDictionaryServiceClient dictionaryServiceClient,
        IAdmissionServiceClient admissionServiceClient)
    {
        _userServiceClient = userServiceClient;
        _documentServiceClient = documentServiceClient;
        _dictionaryServiceClient = dictionaryServiceClient;
        _admissionServiceClient = admissionServiceClient;
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
            var documentTypes = await _dictionaryServiceClient.GetEducationDocumentTypes();
            
            var isMine = await _admissionServiceClient.IsMine(applicant.Id);
            
            Console.WriteLine("Education Document Types:");
            if (documentTypes != null && documentTypes.Any())
            {
                foreach (var docType in documentTypes)
                {
                    Console.WriteLine($"ID: {docType.Id}, Name: {docType.Name}, Education Level: {(docType.EducationLevel?.Name ?? "N/A")}");
                }
            }
            else
            {
                Console.WriteLine("No document types found.");
            }
            
            var passportTask = await _documentServiceClient.GetPassport(applicantId);
            var educationDocumentTask = await _documentServiceClient.GetEducationDocument(applicantId);
            
            
            var viewModel = new ApplicantDetailsViewModel
            {
                Applicant = applicant,
                Passport = passportTask != null ? passportTask : null,
                EducationDocument = educationDocumentTask != null ? educationDocumentTask : null,
                DocumentTypes = documentTypes,
                IsMine = isMine
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

    [HttpPost]
    [Authorize(Roles = "Administrator, Manager, SeniorManager")]
    public async Task<IActionResult> EditUserData(EditUserDto dto, Guid applicantId)
    {
        if (dto.Birthday.Kind == DateTimeKind.Unspecified)
        {
            dto.Birthday = DateTime.SpecifyKind(dto.Birthday, DateTimeKind.Utc);
        }
        else if (dto.Birthday.Kind == DateTimeKind.Local)
        {
            dto.Birthday = dto.Birthday.ToUniversalTime();
        }
        
        if (!ModelState.IsValid)
        {
            TempData["Error"] = "Ошибка при изменении данных";
            return RedirectToAction("Details", new {applicantId = applicantId});
        }
        
        var result = await _userServiceClient.EditUserData(dto, applicantId);
        if (result)
        {
            TempData["Success"] = "Данные абитуриента успешно обновлены.";
            return RedirectToAction("Details", new {applicantId = applicantId});
        }
        
        return RedirectToAction("Details", new {applicantId = applicantId});
    }
    
    [HttpPost]
    [Authorize(Roles = "Administrator, Manager, SeniorManager")]
    public async Task<IActionResult> EditPassport(EditPassportDto dto, Guid applicantId)
    {
        if (dto.WhenIssued.Kind == DateTimeKind.Unspecified)
        {
            dto.WhenIssued = DateTime.SpecifyKind(dto.WhenIssued, DateTimeKind.Utc);
        }
        else if (dto.WhenIssued.Kind == DateTimeKind.Local)
        {
            dto.WhenIssued = dto.WhenIssued.ToUniversalTime();
        }
        
        if (!ModelState.IsValid)
        {
            TempData["Error"] = "Ошибка при изменении паспортных данных";
            return RedirectToAction("Details", new {applicantId = applicantId});
        }

        
        var result = await _documentServiceClient.EditPassport(dto, applicantId);
            
        if (result)
        {
            TempData["Success"] = "Данные абитуриента успешно обновлены.";
            return RedirectToAction("Details", new {applicantId = applicantId});
        }
        
        return RedirectToAction("Details", new {applicantId = applicantId});
    }
    
    [HttpPost]
    [Authorize(Roles = "Administrator, Manager, SeniorManager")]
    public async Task<IActionResult> EditEducationDocument(EditEducationDocumentDto dto, Guid applicantId)
    {
        if (!ModelState.IsValid)
        {
            TempData["Error"] = "Ошибка при изменении данных документа об образовании";
            return RedirectToAction("Details", new {applicantId = applicantId});
        }

        var result = await _documentServiceClient.EditEducationDocument(dto, applicantId);
            
        if (result)
        {
            TempData["Success"] = "Данные абитуриента успешно обновлены.";
            return RedirectToAction("Details", new {applicantId = applicantId});
        }
        
        return RedirectToAction("Details", new {applicantId = applicantId});
    }
    
    [HttpPost]
    [Authorize(Roles = "Administrator, Manager, SeniorManager")]
    public async Task<IActionResult> DeleteFile(Guid scanId, Guid applicantId)
    {
        if (scanId == Guid.Empty || applicantId == Guid.Empty)
        {
            TempData["Error"] = "Неверный идентификатор файла или абитуриента.";
            return RedirectToAction("Details", new { applicantId });
        }

        var result = await _documentServiceClient.DeleteFile(scanId, applicantId);
        if (result)
        {
            TempData["Success"] = "Файл успешно удалён.";
        }
        else
        {
            TempData["Error"] = "Не удалось удалить файл.";
        }

        return RedirectToAction("Details", new { applicantId });
    }
    
    [HttpPost]
    [Authorize(Roles = "Administrator, Manager, SeniorManager")]
    public async Task<IActionResult> UploadFile(CreateScanDto dto, Guid documentId, Guid applicantId)
    {
        if (documentId == Guid.Empty || applicantId == Guid.Empty)
        {
            TempData["Error"] = "Неверный идентификатор документа или абитуриента.";
            return RedirectToAction("Details", new { applicantId });
        }

        if (!ModelState.IsValid || dto.File == null)
        {
            TempData["Error"] = "Ошибка при загрузке файла: выберите корректный файл.";
            return RedirectToAction("Details", new { applicantId });
        }

        var result = await _documentServiceClient.UploadFile(dto, documentId, applicantId);
        if (result)
        {
            TempData["Success"] = "Файл успешно загружен.";
        }
        else
        {
            TempData["Error"] = "Не удалось загрузить файл.";
        }

        return RedirectToAction("Details", new { applicantId });
    }
}