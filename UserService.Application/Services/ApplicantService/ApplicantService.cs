using Contract.Application.Common;
using Contract.Application.Exceptions;
using Contract.Domain.Enums;
using UserService.Application.Dtos.Requests;
using UserService.Application.Dtos.Responses;
using UserService.Application.Interfaces;
using UserService.Domain.Entities;
using UserService.Domain.IRepositories;

namespace UserService.Application.Services.ApplicantService;

public class ApplicantService : IApplicantService
{
    private readonly IUserRepository _userRepository;
    private readonly IApplicantRepository _applicantRepository;
    private readonly IManagerRepository _managerRepository;
    private readonly IPasswordHasherService _passwordHasherService;

    public ApplicantService(
        IUserRepository userRepository,
        IApplicantRepository applicantRepository,
        IManagerRepository managerRepository,
        IPasswordHasherService passwordHasherService)
    {
        _userRepository = userRepository;
        _applicantRepository = applicantRepository;
        _managerRepository = managerRepository;
        _passwordHasherService = passwordHasherService;
    }

    public async Task<GenericResult<ApplicantDto>> GetProfile(Guid userId)
    {
        var applicantDb = await _applicantRepository.GetByUserIdAsync(userId);

        if (applicantDb == null)
        {
            throw new NotFoundException("Applicant not found.");
        }

        var applicantDto = new ApplicantDto
        {
            Id = applicantDb.Id,
            Email = applicantDb.User.Email,
            FullName = applicantDb.FullName,
            BirthDay = applicantDb.BirthDay,
            Gender = applicantDb.Gender,
            Citizenship = applicantDb.Citizenship,
            PhoneNumber = applicantDb.PhoneNumber,
        };

        return GenericResult<ApplicantDto>.Success(applicantDto);
    }

    public async Task<Result> EditProfile(EditUserDto dto, Guid userId)
    {
        var applicantDb = await _applicantRepository.GetByUserIdAsync(userId);

        if (applicantDb == null)
        {
            throw new NotFoundException("Applicant not found.");
        }

        var existingUser = await _userRepository.GetByEmailAsync(dto.Email);
        if (existingUser != null && existingUser.Id != userId)
        {
            throw new ValidationException("Email already exists.");
        }

        applicantDb.User.Email = dto.Email;

        applicantDb.FullName = dto.FullName;
        applicantDb.BirthDay = dto.Birthday;
        applicantDb.Gender = dto.Gender;
        applicantDb.Citizenship = dto.Citizenship;
        applicantDb.PhoneNumber = dto.PhoneNumber;

        await _applicantRepository.SaveChangesAsync();
        return Result.Success();
    }

    public async Task<Result> ResetPassword(ResetPasswordDto dto, Guid userId)
    {
        var userDb = await _userRepository.GetByIdAsync(userId);

        if (userDb == null)
        {
            throw new NotFoundException("User not found.");
        }

        if (!_passwordHasherService.VerifyPassword(dto.OldPassword, userDb.HashedPassword))
        {
            throw new ValidationException("Old password is incorrect.");
        }

        var newHashedPassword = _passwordHasherService.HashPassword(dto.NewPassword);

        userDb.HashedPassword = newHashedPassword;

        await _applicantRepository.SaveChangesAsync();

        return Result.Success();
    }

    public async Task<Role> GetRoleByUserId(Guid userId)
    {
        var userDb = await _userRepository.GetByIdAsync(userId);

        if (userDb == null)
        {
            throw new NotFoundException("User not found.");
        }

        return userDb.Role;
    }

    public async Task<string> GetEmailByUserId(Guid userId)
    {
        var userDb = await _userRepository.GetByIdAsync(userId);
        
        if (userDb == null)
        {
            throw new NotFoundException("User not found.");
        }
        
        return userDb.Email;
    }

    public async Task<List<ApplicantDto>> GetAllApplicants()
    {
        var applicants = await _userRepository.GetAllApplicants();

        var applicantsDto = applicants
            .Select(a => new ApplicantDto
            {
                Id = a.Id,
                Email = a.User.Email,
                FullName = a.FullName,
                BirthDay = a.BirthDay,
                Gender = a.Gender,
                Citizenship = a.Citizenship,
                PhoneNumber = a.PhoneNumber
            });

        return applicantsDto.ToList();
    }

    public async Task<List<ManagerDto>> GetAllManagers()
    {
        var managers = await _userRepository.GetAllManagers();

        var managersDto = managers
            .Select(m => new ManagerDto
            {
                Id = m.Id,
                Email = m.User.Email,
                FullName = m.FullName,
                Role = m.User.Role,
            });

        return managersDto.ToList();
    }

    public async Task AssignRoleTo(Guid userId, Role role)
    {
        if (role == Role.Administrator)
        {
            throw new ForbiddenAccessException("You cannot assign administrator roles.");
        }
        
        var userDb = await _userRepository.GetByIdAsync(userId);
        
        if (userDb == null)
        {
            throw new NotFoundException("User not found.");
        }
        
        if (userDb.Role == Role.Administrator)
        {
            throw new ForbiddenAccessException("You cannot change administrator role.");
        }
        
        if (userDb.Role == role)
        {
            return;
        }
        
        if (userDb.Role == Role.Applicant && (role == Role.Manager || role == Role.SeniorManager))
        {
            var applicant = await _applicantRepository.GetByUserIdAsync(userId);
            var fullName = applicant.FullName;
            if (applicant != null)
            {
                await _applicantRepository.DeleteAsync(applicant.Id);
            }
            
            var manager = new Manager
            {
                Id = userDb.Id,
                FullName = fullName,
                User = userDb,
            };
            await _managerRepository.AddAsync(manager);
        }
        
        else if ((userDb.Role == Role.Manager || userDb.Role == Role.SeniorManager) && role == Role.Applicant)
        {
            var manager = await _managerRepository.GetByIdAsync(userId);
            var fullName = manager.FullName;
            if (manager != null)
            {
                await _managerRepository.DeleteAsync(manager.Id);
            }

            var applicant = new Applicant
            {
                Id = userDb.Id,
                User = userDb,
                FullName = fullName,
                Gender = Gender.Male,
                Citizenship = "Не указан",
                PhoneNumber = "Не указан",
                BirthDay = DateTime.UtcNow,
            };
            await _applicantRepository.AddAsync(applicant);
        }

        else if ((userDb.Role == Role.SeniorManager && role == Role.Manager) ||
                 (userDb.Role == Role.Manager && role == Role.SeniorManager))
        {
        }
        else
        {
            throw new InvalidOperationException($"Invalid role transition from {userDb.Role} to {role}");
        }
        
        userDb.Role = role;

        await _userRepository.SaveChangesAsync();
    }
}