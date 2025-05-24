using AdmissionService.Application.Dtos.Responses;
using AdmissionService.Application.Services.DictionaryServiceClient;
using AdmissionService.Domain.Entities;
using AdmissionService.Domain.IRepositories;
using Contract.Application.Exceptions;
using Contract.Domain.Enums;

namespace AdmissionService.Application.Services.AdmissionService;

public class AdmissionService : IAdmissionService
{
    private readonly IAdmissionSettingRepository _admissionSettingRepository;
    private readonly IStudentAdmissionRepository _studentAdmissionRepository; 
    private readonly IDictionaryServiceClient _dictionaryServiceClient;
    public AdmissionService(
        IAdmissionSettingRepository admissionSettingRepository,
        IStudentAdmissionRepository studentAdmissionRepository,
        IDictionaryServiceClient dictionaryServiceClient)
    {
        _admissionSettingRepository = admissionSettingRepository;
        _studentAdmissionRepository = studentAdmissionRepository;
        _dictionaryServiceClient = dictionaryServiceClient;
    }

    public async Task<Guid> CreateAdmission(Guid userId)
    {
        if (!await _admissionSettingRepository.IsAdmissionOpenAsync())
        {
            throw new ForbiddenAccessException("University Admission is not currently available");
        }

        if (await _studentAdmissionRepository.IsStudentAdmissionExists(userId))
        {
            throw new BadRequestException("Admission already exists");
        }

        var seasonId = _admissionSettingRepository.GetAdmissionSeasonIdAsync();
        var newAdmission = new StudentAdmission
        {
            Id = Guid.NewGuid(),
            CreatedTime = DateTime.UtcNow,
            ApplicantId = userId,
            AdmissionSeasonId = seasonId,
            Status = AdmissionStatus.Created,
        };
        
        await _studentAdmissionRepository.AddStudentAdmissionAsync(newAdmission);
        
        return  newAdmission.Id;
    }

    public async Task<List<ShortAdmissionDto>> GetMyAdmissions(Guid userId)
    {
        var admissionsDb = await _studentAdmissionRepository.GetStudentAdmissions(userId);
        
        var admissionsDto = new List<ShortAdmissionDto>();

        foreach (var admission in admissionsDb)
        {
            var admissionDto = new ShortAdmissionDto
            {
                Id = admission.Id,
                Status = admission.Status,
                IsManagerExist = (admission.ManagerId != null),
                AdmissionSeasonId = admission.AdmissionSeasonId,
                ProgramsCount = admission.AdmissionPrograms.Count()
            };
            
            admissionsDto.Add(admissionDto);
        }
        
        return admissionsDto;
    }

    public async Task<StudentAdmissionDto> GetAdmissionById(Guid admissionId, Guid userId)
    {
        var existingAdmission = await _studentAdmissionRepository.GetStudentAdmissionByIdAsync(admissionId, userId);
        if (existingAdmission == null)
        {
            throw new NotFoundException("Admission not found or not created yet");
        }

        var programs = await loadPrograms(existingAdmission);
        var studentAdmissionDto = new StudentAdmissionDto
        {
            Id = existingAdmission.Id,
            CreatedTime = existingAdmission.CreatedTime,
            ModifiedTime = existingAdmission.ModifiedTime,
            IsManagerExist = (existingAdmission.ManagerId != null),
            Status = existingAdmission.Status,
            AdmissionPrograms = programs,
        };
        
        return studentAdmissionDto; 
    }

    private async Task<List<AdmissionProgramDto>> loadPrograms(StudentAdmission admission)
    {
        var programs = new List<AdmissionProgramDto>();

        if (admission.AdmissionPrograms == null)
        {
            return programs;
        }

        foreach (var program in admission.AdmissionPrograms)
        {
            var educationProgramDto = await _dictionaryServiceClient.GetEducationProgramById(program.Id);
            var programDto = new AdmissionProgramDto
            {
                Id = program.Id,
                CreatedTime = program.CreatedTime,
                ModifiedTime = program.ModifiedTime,
                Priority = program.Priority,
                EducationProgram = educationProgramDto
            };
            
            programs.Add(programDto);
        }
        
        return programs;
    }
}