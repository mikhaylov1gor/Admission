using System.Transactions;
using AdmissionService.Application.Dtos.Requests;
using AdmissionService.Application.Dtos.Responses;
using AdmissionService.Application.Services.DictionaryServiceClient;
using AdmissionService.Application.Services.DocumentServiceClient;
using AdmissionService.Domain.Entities;
using AdmissionService.Domain.IRepositories;
using Contract.Application.Exceptions;
using Contract.Application.Validations;

namespace AdmissionService.Application.Services.ProgramService;

public class ProgramService : IProgramService
{
    private readonly int MaxPrograms = Priority.MaxPriority;
    
    private readonly IAdmissionSettingRepository _admissionSettingRepository;
    private readonly IStudentAdmissionRepository _studentAdmissionRepository;
    private readonly IDictionaryServiceClient _dictionaryServiceClient;
    private readonly IDocumentServiceClient _documentServiceClient;
    
    public ProgramService(
        IAdmissionSettingRepository admissionSettingRepository,
        IStudentAdmissionRepository studentAdmissionRepository,
        IDictionaryServiceClient dictionaryServiceClient,
        IDocumentServiceClient documentServiceClient)
    {
        _admissionSettingRepository = admissionSettingRepository;
        _studentAdmissionRepository = studentAdmissionRepository;
        _dictionaryServiceClient = dictionaryServiceClient;
        _documentServiceClient = documentServiceClient;
    }

    public async Task AddProgramToAdmission(AddProgramDto dto, Guid userId)
    {
        if (!await _admissionSettingRepository.IsAdmissionOpenAsync())
        {
            throw new ForbiddenAccessException("Admission is currently unavailable");
        }
        
        var currentAdmission = await _studentAdmissionRepository.GetCurrentStudentAdmissionAsync(userId);

        if (currentAdmission == null)
        {
            throw new BadRequestException("User don't have an admission");
        }

        if (currentAdmission.AdmissionPrograms.Count >= MaxPrograms)
        {
            throw new BadRequestException($"Max programs reached: ({MaxPrograms})");
        }
        
        var program = await _dictionaryServiceClient.GetEducationProgramById(dto.EducationProgramId);

        if (program == null)
        {
            throw new NotFoundException($"Education program with id: {dto.EducationProgramId} does not exist");
        }

        if (IsPriorityExists(dto.Priority, currentAdmission))
        {
            throw new BadRequestException($"Program with priority {dto.Priority} already exists");
        }

        if (!await IsEducationLevelsOneStage(program.EducationLevel.Name, currentAdmission))
        {
            throw new BadRequestException($"Admission Programs should relate to one level of study");
        }
        
        var documentEducationLevelName = await _documentServiceClient.GetEducationDocumentLevelName(dto.EducationProgramId);

        if (documentEducationLevelName != null)
        {
            if (!await IsEducationLevelsOneStage(documentEducationLevelName, currentAdmission))
            {
                throw new BadRequestException("your level of education does not allow you to add this program.");
            }
        }

        var newAdmissionProgram = new AdmissionProgram
        {
            Id = dto.EducationProgramId,
            StudentAdmissionId = currentAdmission.Id,
            CreatedTime = DateTime.UtcNow,

            Priority = dto.Priority,
            StudentAdmission = currentAdmission,
        };
        
        await _studentAdmissionRepository.AddAdmissionProgramAsync(newAdmissionProgram);
        await _studentAdmissionRepository.SaveChangesAsync();
    }

    public async Task EditAdmissionPrograms(EditProgramsDto dto, Guid userId)
    {
        return;
    }

    public async Task RemoveProgramFromAdmission(Guid programId, Guid userId)
    {
        if (!await _admissionSettingRepository.IsAdmissionOpenAsync())
        {
            throw new ForbiddenAccessException("Admission is currently unavailable");
        }
        
        var currentAdmission = await _studentAdmissionRepository.GetCurrentStudentAdmissionAsync(userId);

        if (currentAdmission == null)
        {
            throw new BadRequestException("User don't have an admission");
        }
        
        var program = currentAdmission.AdmissionPrograms.FirstOrDefault(p => p.Id == programId);

        if (program == null)
        {
            throw new NotFoundException($"Admission Program with id: {programId} does not exist");
        }
        
        await _studentAdmissionRepository.DeleteStudentAdmissionProgramByIdAsync(programId, userId);
    }

    private bool IsPriorityExists(int priority, StudentAdmission admission)
    {
        if (admission.AdmissionPrograms.Any(p => p.Priority == priority))
        {
            return false;
        }
        return true;
    }

    private async Task<bool> IsEducationLevelsOneStage(string newProgramEducationLevelName, StudentAdmission admission)
    {
        if (admission.AdmissionPrograms.Count == 0)
        {
            return true;
        }
        
        var existingLevelId = admission.AdmissionPrograms.First().Id;
        
        var existingProgram = await _dictionaryServiceClient.GetEducationLevelById(existingLevelId);

        var existingProgramName = existingProgram.Name;
        
        if (newProgramEducationLevelName == existingProgramName)
        {
            return true;
        }
        
        return AreEducationLevelsCompatible(existingProgramName, newProgramEducationLevelName);

        return false;
    }
    
    private bool AreEducationLevelsCompatible(string existingLevelName, string newLevelName)
    {
        var compatibleTransitions = new Dictionary<string, List<string>>
        {
            // Бакалавриат → Специалитет
            { 
                "Бакалавриат", 
                new List<string> { "Специалитет" } 
            },
            // Специалитет → Магистратура
            { 
                "Специалитет", 
                new List<string> { "Магистратура" } 
            },
            // Магистратура → Аспирантура
            { 
                "Магистратура", 
                new List<string> { "Аспирантура" } 
            }
        };

        return compatibleTransitions.TryGetValue(existingLevelName, out var allowedNextLevels) 
               && allowedNextLevels.Contains(newLevelName);
    }
}