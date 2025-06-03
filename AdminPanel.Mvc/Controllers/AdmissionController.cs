using AdminPanel.Mvc.Models.Admissions;
using AdminPanel.Mvc.Services.AdmissionServiceClient;
using AdmissionService.Application.Dtos.Requests;
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
            var isMine = await _admissionServiceClient.IsMineAdmission(admissionId);
            
            Console.WriteLine(isMine);
            var viewModel = new AdmissionDetailsViewModel
            {
                StudentAdmission = admission,
                ApplicantId = applicantId,
                IsMine = isMine
            };
            
            return View(viewModel);

        }
        catch (Exception ex)
        {
            TempData["Error"] = "Ошибка при получении данных";
            return RedirectToAction("Admissions");
        }
    }

    [HttpPost]
    [Authorize(Roles = "Manager,SeniorManager, Administrator")]
    public async Task<IActionResult> ChangeStatus(Guid admissionId, AdmissionStatus newStatus)
    {
        try
        {
            var result = await _admissionServiceClient.ChangeStatusAsync(admissionId, newStatus);
            if (result)
            {
                TempData["StatusChanged"] = true;
                return RedirectToAction("Details", new { admissionId });
            }

            return BadRequest("Не удалось изменить статус");
        }
        catch (Exception ex)
        {
            throw;
        }
    }

    [HttpPost]
    [Authorize(Roles = "Manager,SeniorManager, Administrator")]
    public async Task<IActionResult> DeleteProgram(Guid admissionId, Guid programId)
    {
        try
        {
            var result = await _admissionServiceClient.RemoveProgramFromAdmissionAsync(admissionId, programId);
            if (result)
            {
                TempData["ProgramDeleted"] = true;
                return RedirectToAction("Details", new { admissionId });
            }
            return BadRequest("Не удалось удалить программу");
        }
        catch (Exception ex)
        {
            throw;
        }
    }

    [HttpPost]
    [Authorize(Roles = "Manager,SeniorManager, Administrator")]
    public async Task<IActionResult> ChangeProgramPriorities(Guid admissionId, [FromForm] EditProgramsDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage);
                
                TempData["PriorityError"] = string.Join("; ", errors);
                return RedirectToAction("Details", new { admissionId });
            }
            
            var result = await _admissionServiceClient.ChangePriorities(admissionId, dto);
            
            if (result)
            {
                TempData["PrioritiesUpdated"] = true;
                return RedirectToAction("Details", new { admissionId });
            }

            return BadRequest("Не удалось изменить приоритеты");
        }
        catch (Exception ex)
        {
            throw;
        }
    }
}