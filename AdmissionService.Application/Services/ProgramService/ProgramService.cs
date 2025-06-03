using System.Transactions;
using AdmissionService.Application.Dtos.Requests;
using AdmissionService.Application.Dtos.Responses;
using AdmissionService.Application.Services.DictionaryServiceClient;
using AdmissionService.Application.Services.DocumentServiceClient;
using AdmissionService.Domain.Entities;
using AdmissionService.Domain.IRepositories;
using Contract.Application.Exceptions;
using Contract.Application.Validations;
using Contract.Domain.Enums;

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

        if (currentAdmission.Status == AdmissionStatus.Closed)
        {
            throw new ForbiddenAccessException("Your admission is closed");
        }
        
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

        if (IsProgramExists(dto.EducationProgramId, currentAdmission))
        {
            throw new BadRequestException($"Program with id: {dto.EducationProgramId} already exists");
        }

        if (IsPriorityExists(dto.Priority, currentAdmission))
        {
            throw new BadRequestException($"Program with priority {dto.Priority} already exists");
        }

        if (!await IsEducationLevelsOneStage(program.EducationLevel.Name, currentAdmission))
        {
            throw new BadRequestException($"Admission Programs should relate to one level of study");
        }

        var documentEducationLevelName =
            await _documentServiceClient.GetEducationDocumentLevelName(dto.EducationProgramId);

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
        if (!await _admissionSettingRepository.IsAdmissionOpenAsync())
        {
            throw new ForbiddenAccessException("Admission is currently unavailable");
        }

        var currentAdmission = await _studentAdmissionRepository.GetCurrentStudentAdmissionAsync(userId);

        if (currentAdmission.Status == AdmissionStatus.Closed)
        {
            throw new ForbiddenAccessException("Your admission is closed");
        }

        if (currentAdmission == null)
        {
            throw new BadRequestException("User don't have current admission");
        }

        if (!IsPrioritiesUnique(dto))
        {
            throw new BadRequestException($"Priorities are not unique");
        }

        if (!IsProgramsExistsInAdmission(dto, currentAdmission))
        {
            throw new BadRequestException("Some programs are not exists in your admission or not exists in dto");
        }
        
        var programDictionary = currentAdmission.AdmissionPrograms.ToDictionary(p => p.Id);
        foreach (var programDto in dto.EditPrograms)
        {
            if (programDictionary.TryGetValue(programDto.EducationProgramId, out var program))
            {
                program.Priority = programDto.Priority;
                await _studentAdmissionRepository.UpdateAdmissionProgram(program);
            }
        }
        
        await _studentAdmissionRepository.SaveChangesAsync();
    }

    public async Task EditAdmissionProgramsForManager(Guid admissionId, EditProgramsDto dto, Guid managerId, 
        bool isGigaWorker)
    {
        var admission = await  _studentAdmissionRepository.GetStudentAdmissionByIdAsync(admissionId, managerId, true);
        if (admission == null)
        {
            throw new NotFoundException("Admission not found or not created");
        }
        
        if (!IsPrioritiesUnique(dto))
        {
            throw new BadRequestException($"Priorities are not unique");
        }

        if (!IsProgramsExistsInAdmission(dto, admission))
        {
            throw new BadRequestException("Some programs are not exists in your admission or not exists in dto");
        }
        
        var programDictionary = admission.AdmissionPrograms.ToDictionary(p => p.Id);
        foreach (var programDto in dto.EditPrograms)
        {
            if (programDictionary.TryGetValue(programDto.EducationProgramId, out var program))
            {
                program.Priority = programDto.Priority;
                await _studentAdmissionRepository.UpdateAdmissionProgram(program);
            }
        }
        
        await _studentAdmissionRepository.SaveChangesAsync();
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

        await _studentAdmissionRepository.DeleteStudentAdmissionProgramByIdAsync(currentAdmission.Id, programId);
    }

    public async Task RemoveProgramForManager(Guid admissionId, Guid programId, Guid managerId, bool isGigaWorker)
    {
        var admission = await  _studentAdmissionRepository.GetStudentAdmissionByIdAsync(admissionId, managerId, true);
        if (admission == null)
        {
            throw new NotFoundException("Admission not found or not created");
        }

        if (!isGigaWorker && admission.ManagerId != managerId)
        {
            throw new ForbiddenAccessException("You cannot manage this admission");
        }
        
        var program = admission.AdmissionPrograms.FirstOrDefault(p => p.Id == programId);

        if (program == null)
        {
            throw new NotFoundException($"Admission Program with id: {programId} does not exist");
        }

        await _studentAdmissionRepository.DeleteStudentAdmissionProgramByIdAsync(admission.Id, programId);
    }

    private bool IsPrioritiesUnique(EditProgramsDto dto)
    {
        var priorities = dto.EditPrograms.Select(p => p.Priority).ToList();
        var uniquePriorities = new HashSet<int>(priorities);
    
        return priorities.Count == uniquePriorities.Count;
    }

    private bool IsProgramsExistsInAdmission(EditProgramsDto dto, StudentAdmission currentAdmission)
    {
        if (dto?.EditPrograms == null || currentAdmission?.AdmissionPrograms == null)
            return false;

        if (dto?.EditPrograms.Count() != currentAdmission.AdmissionPrograms.Count())
            return false;
        
        var dtoProgramIds = dto.EditPrograms.Select(p => p.EducationProgramId).ToHashSet();
        
        var admissionProgramIds = currentAdmission.AdmissionPrograms.Select(p => p.Id).ToHashSet();

        return dtoProgramIds.All(id => admissionProgramIds.Contains(id));
    }
    private bool IsPriorityExists(int priority, StudentAdmission admission)
    {
        if (admission.AdmissionPrograms.Any(p => p.Priority == priority))
        {
            return true;
        }

        return false;
    }

    private bool IsProgramExists(Guid programId, StudentAdmission admission)
    {
        if (admission.AdmissionPrograms.Any(p => p.Id == programId))
        {
            return true;
        }

        return false;
    }

    private async Task<bool> IsEducationLevelsOneStage(string newProgramEducationLevelName, StudentAdmission admission)
    {
        if (admission.AdmissionPrograms.Count == 0)
        {
            return true;
        }
        
        var firstProgram = admission.AdmissionPrograms.First();
        
        var existingProgram = await _dictionaryServiceClient.GetEducationProgramById(firstProgram.Id);
        
        if (existingProgram == null)
        {
            return false;
        }

        if (newProgramEducationLevelName == existingProgram.EducationLevel.Name)
        {
            return true;
        }
        
        return AreEducationLevelsCompatible(existingProgram.EducationLevel.Name, newProgramEducationLevelName);
    }

    private bool AreEducationLevelsCompatible(string existingLevelName, string newLevelName)
    {
        var compatibleTransitions = new Dictionary<string, List<string>>
        {
            // Бакалавриат → Бакалавриат, Специалитет
            {
                "Бакалавриат",
                new List<string> { "Бакалавриат", "Специалитет" }
            },
            // Специалитет → Специалитет, Магистратура
            {
                "Специалитет",
                new List<string> { "Специалитет", "Магистратура" }
            },
            // Магистратура → Магистратура, Аспирантура
            {
                "Магистратура",
                new List<string> { "Магистратура", "Аспирантура" }
            }
        };
        
        if (!compatibleTransitions.TryGetValue(existingLevelName, out var allowedNextLevels))
        {
            return false;
        }
        
        return allowedNextLevels.Contains(newLevelName);
    }
}