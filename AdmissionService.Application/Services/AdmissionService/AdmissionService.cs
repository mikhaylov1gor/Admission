using AdmissionService.Application.Dtos.Responses;
using AdmissionService.Application.Services.DictionaryServiceClient;
using AdmissionService.Application.Services.UserServiceClient;
using AdmissionService.Domain.Entities;
using AdmissionService.Domain.IRepositories;
using Contract.Application.Exceptions;
using Contract.Domain.Enums;
using Contract.Dtos.Dtos.Requests;
using Contract.Dtos.Dtos.Responses;
using NotificationService.Api.Models;
using NotificationService.Api.Services.EmailPublisherService;
using StudentAdmissionDto = AdmissionService.Application.Dtos.Responses.StudentAdmissionDto;

namespace AdmissionService.Application.Services.AdmissionService;

public class AdmissionService : IAdmissionService
{
    private readonly IAdmissionSettingRepository _admissionSettingRepository;
    private readonly IStudentAdmissionRepository _studentAdmissionRepository; 
    private readonly IDictionaryServiceClient _dictionaryServiceClient;
    private readonly IUserServiceClient _userServiceClient;
    private readonly IEmailPublisher _emailPublisher;
    public AdmissionService(
        IAdmissionSettingRepository admissionSettingRepository,
        IStudentAdmissionRepository studentAdmissionRepository,
        IDictionaryServiceClient dictionaryServiceClient,
        IUserServiceClient userServiceClient,
        IEmailPublisher emailPublisher)
    {
        _admissionSettingRepository = admissionSettingRepository;
        _studentAdmissionRepository = studentAdmissionRepository;
        _dictionaryServiceClient = dictionaryServiceClient;
        _userServiceClient = userServiceClient;
        _emailPublisher = emailPublisher;
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

    public async Task<StudentAdmissionDto> GetAdmissionById(Guid admissionId, Guid userId, bool isWorker)
    {
        var existingAdmission = await _studentAdmissionRepository.GetStudentAdmissionByIdAsync(admissionId, userId, isWorker);
        if (existingAdmission == null)
        {
            throw new NotFoundException("Admission not found or not created yet");
        }

        var programs = await LoadPrograms(existingAdmission);
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

    public async Task<bool> IsAdmissionOpen()
    {
        return await _admissionSettingRepository.IsAdmissionOpenAsync();
    }

    public async Task<bool> IsMine(Guid admissionId, Guid userId)
    {
        var admission = await  _studentAdmissionRepository.GetStudentAdmissionByIdAsync(admissionId, userId, true);

        if (admission == null)
        {
            throw new NotFoundException("Admission not found or not created");
        }

        if (admission.ManagerId == userId)
        {
            return true;
        }

        return false;
    }

    public async Task ChangeAdmissionStatus(Guid admissionId, AdmissionStatus newStatus, Guid managerId, bool isGigaWorker)
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
        
        admission.Status = newStatus;
        admission.ModifiedTime =  DateTime.UtcNow;
        await _studentAdmissionRepository.SaveChangesAsync();
        
        await SendEmailToApplicant(admission.ApplicantId, newStatus);
    }
    
    public async Task<AdmissionsDto> GetAdmissions(GetAdmissionsDto dto, Guid workerId)
    {
        var allAdmissions = await _studentAdmissionRepository.GetAllAdmissions();
        
        if (dto.IsManagerExists == true)
        {
            allAdmissions = allAdmissions.Where(a => a.ManagerId != null).ToList();
        }

        if (dto.AdmissionStatus != null)
        {
            allAdmissions = allAdmissions.Where(a => a.Status == dto.AdmissionStatus).ToList();
        }

        if (dto.OnlyMine)
        {
            allAdmissions = allAdmissions.Where(a => a.ManagerId == workerId).ToList();
        }

        switch (dto.Sorting)
        {
            case Sorting.ModifiedAsc:
                allAdmissions = allAdmissions.OrderBy(a => a.ModifiedTime).ToList();
                break;
            
            case Sorting.ModifiedDesc:
                allAdmissions = allAdmissions.OrderByDescending(a => a.ModifiedTime).ToList();
                break;
        }
        
        
        var pagesCount = (int)Math.Ceiling((double)allAdmissions.Count() / dto.Size);
        allAdmissions = allAdmissions
            .Skip((dto.Page - 1) * dto.Size)
            .Take(dto.Size)
            .ToList();

        var admissionsDto = new List<AdmissionDto>();
        
        admissionsDto = allAdmissions
            .Select(aa => new AdmissionDto
            {
                AdmissionId = aa.Id,
                Status = aa.Status,
                ApplicantId = aa.ApplicantId,
                IsManagerExists = (aa.ManagerId != null),
                ManagerId = aa.ManagerId,
                IsMine = (aa.ManagerId == workerId),
                IsEditable = aa.Status != AdmissionStatus.Closed && aa.ManagerId == workerId
            })
            .ToList();

        var pagination = new Pagination
        {
            Current = dto.Page,
            Size = dto.Size,
            Count = pagesCount,
        };
        
        return new AdmissionsDto{Pagination = pagination, StudentAdmissions =  admissionsDto};
    }

    public async Task TakeUntakeAdmission(Guid admissionId, Guid workerId, Guid userId)
    {
        var studentAdmission = await _studentAdmissionRepository.GetStudentAdmissionByIdAsync(admissionId, userId, true);

        if (studentAdmission == null)
        {
            throw new NotFoundException("Admission not found or not created yet");
        }

        if (studentAdmission.ManagerId != null && studentAdmission.ManagerId != workerId)
        {
            throw new ForbiddenAccessException("This admission already taken by another manager");
        }

        if (studentAdmission.ManagerId == workerId)
        {
            studentAdmission.ManagerId = null;
            
            if (studentAdmission.Status == AdmissionStatus.InProgress)
                studentAdmission.Status = AdmissionStatus.Created;
        }
        else 
        {
            studentAdmission.ManagerId = workerId;
            studentAdmission.Status = AdmissionStatus.InProgress;
        }
    
        studentAdmission.ModifiedTime = DateTime.UtcNow;

        await _studentAdmissionRepository.SaveChangesAsync();
        
        await SendEmailToApplicant(studentAdmission.ApplicantId, studentAdmission.Status);
        if (studentAdmission.Status == AdmissionStatus.InProgress)
        {
            await SendEmailToManager(workerId);   
        }
    }

    private async Task<List<AdmissionProgramDto>> LoadPrograms(StudentAdmission admission)
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
    
    public async Task AssignManager(Guid admissionId, Guid managerId)
    {
        var studentAdmission = await _studentAdmissionRepository.GetStudentAdmissionByIdAsync(admissionId, managerId, true);

        if (studentAdmission == null)
        {
            throw new NotFoundException("Admission not found or not created yet");
        }

        if (studentAdmission.ManagerId != null)
        {
            throw new BadRequestException("Admission already assigned");
        }
        
        studentAdmission.ManagerId = managerId;
        studentAdmission.ModifiedTime = DateTime.UtcNow;
        studentAdmission.Status = AdmissionStatus.InProgress;
        
        await _studentAdmissionRepository.SaveChangesAsync();
        
        await SendEmailToApplicant(studentAdmission.ApplicantId, studentAdmission.Status);
        await SendEmailToManager(managerId);
    }

    public async Task<bool> IsMineApplicantByAdmission(Guid applicantId, bool isGigaWorker, Guid UserId)
    {
        if (isGigaWorker)
        {
            return true;
        }

        var currentAdmission = await _studentAdmissionRepository.GetCurrentStudentAdmissionAsync(applicantId);

        if (currentAdmission == null)
        {
            throw new NotFoundException("Admission not found or not created yet");
        }

        if (currentAdmission.ManagerId == UserId)
        {
            return true;
        }

        return false;
    }
    
    private async Task SendEmailToApplicant(Guid userId, AdmissionStatus newStatus)
    {
        var email = await _userServiceClient.GetUserEmail(userId);

        Console.WriteLine($"email: {email}");
        if (email == null)
        {
            email = "NotExistEmail@unlucky.com";
        }
        var message = new EmailMessage
        {
            To = email, 
            Subject = "Изменение статуса поступления",
            Body = $"Уведомляем Вас о том, что статус вашего заявления был изменен. Новый статус: {newStatus.ToString()}",
            IsHtml = false
        };

        _emailPublisher.PublishMessage(message);
    }
    
    private async Task SendEmailToManager(Guid userId)
    {
        var email = await _userServiceClient.GetUserEmail(userId);

        if (email == null)
        {
            email = "NotExistEmail@unlucky.com";
        }
        var message = new EmailMessage
        {
            To = email, 
            Subject = "Вам было назначено поступление",
            Body = $"Уведомляем Вас о том, что вам было назначено новое поступление",
            IsHtml = false
        };

        _emailPublisher.PublishMessage(message);
    }
}
