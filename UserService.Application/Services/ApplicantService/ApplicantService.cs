using Contract.Application.Common;
using Contract.Application.Exceptions;
using Contract.Domain.Enums;
using UserService.Application.Dtos.Requests;
using UserService.Application.Dtos.Responses;
using UserService.Application.Interfaces;
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
}