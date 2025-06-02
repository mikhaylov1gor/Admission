using AdminPanel.Mvc.Models.Admissions;
using AdminPanel.Mvc.Services.AdmissionServiceClient;
using AdmissionService.Application.Dtos.Responses;
using AdmissionService.Domain.Entities;
using Contract.Domain.Enums;
using Contract.Dtos.Dtos.Requests;
using Contract.Dtos.Dtos.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdminPanel.Mvc.Controllers;

[Authorize(Roles = "Administrator, Manager, SeniorManager")] 
public class AdmissionController : Controller
{
    private readonly IAdmissionServiceClient _admissionServiceClient;
    
    public AdmissionController(IAdmissionServiceClient admissionServiceClient)
    {
        _admissionServiceClient = admissionServiceClient;

    }
    
    [HttpGet]
    public async Task<IActionResult> Admissions([FromQuery]GetAdmissionsDto filter = null)
    {
        var admissions = await _admissionServiceClient.GetAdmissions(filter);
        
        if (admissions == null)
        {
            TempData["Error"] = "Не удалось загрузить список поступлений.";
            admissions = new AdmissionsDto 
            {
                StudentAdmissions = new List<AdmissionDto>(),
                Pagination = new Pagination()
            };
        }

        var viewModel = new AdmissionsViewModel
        {
            Admissions = admissions,
            Filters = filter
        };

        return View(viewModel);
    }
    
    [HttpPatch("Admission/TakeUntakeAdmission")]
    public async Task<IActionResult> TakeUntakeAdmission(Guid admissionId, Guid applicantId)
    {
        var result = await _admissionServiceClient.TakeUntakeAdmission(admissionId, applicantId);
        return result ? Ok() : StatusCode(500);
    }

    [HttpGet("Admission/Details/{admissionId}")]
    public async Task<IActionResult> Details(Guid admissionId, [FromQuery] Guid applicantId)
    {
        try
        {
            var admission = await _admissionServiceClient.GetStudentAdmission(admissionId);
            
            var viewModel = new AdmissionDetailsViewModel
            {
                StudentAdmission = admission,
                ApplicantId = applicantId
            };
            
            return View(viewModel);

        }
        catch (Exception ex)
        {
            TempData["Error"] = "Ошибка при получении данных";
            return RedirectToAction("Admissions");
        }
    }
}