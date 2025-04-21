using Contract.Application.Common;
using Contract.Application.Exceptions;
using Contract.Domain.Enums;
using UserService.Application.Dtos.Requests;
using UserService.Application.Dtos.Responses;
using UserService.Application.Interfaces;
using UserService.Domain.Entities;
using UserService.Domain.IRepositories;

namespace UserService.Application.Services.AuthService;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IApplicantRepository _applicantRepository;
    private readonly IManagerRepository _managerRepository;
    private readonly IPasswordHasherService _passwordHasherService;

    public AuthService(
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

    public async Task<GenericResult<TokenResponseDto>> RegisterApplicant(RegisterUserDto dto)
    {
        if (await _userRepository.GetByEmailAsync(dto.Email) != null)
        {
            throw new ValidationException("Email already exists.");
        }
        
        var hashedPassword = _passwordHasherService.HashPassword(dto.Password);

        var newUser = new User
        {
            Id = Guid.NewGuid(),
            Email = dto.Email,
            HashedPassword = hashedPassword,
            Role = Role.Applicant
        };
        
        await _userRepository.AddAsync(newUser);

        var newApplicant = new Applicant
        {
            Id = newUser.Id,
            User = newUser,
            
            FullName = dto.FullName,
            BirthDay = dto.Birthday,
            Gender = dto.Gender,
            Citizenship = dto.Citizenship,
            PhoneNumber = dto.PhoneNumber,
        };
        
        await _applicantRepository.AddAsync(newApplicant);
        
        // vremennaya zaglushka
        var tokens = new TokenResponseDto() {AccessToken = "123", RefreshToken = "456"};

        return GenericResult<TokenResponseDto>.Success(tokens);
    }

    public async Task<GenericResult<TokenResponseDto>> Login(LoginCredentialsDto credentials)
    {
        var user = await _userRepository.GetByEmailAsync(credentials.Email);

        if (user == null)
        {
            throw new NotFoundException("User not found.");
        }

        if (!_passwordHasherService.VerifyPassword(credentials.Password, user.HashedPassword))
        {
            throw new ValidationException("Invalid password.");
        }
        
        // vremennaya zaglushka
        var tokens = new TokenResponseDto() {AccessToken = "123", RefreshToken = "456"};

        return GenericResult<TokenResponseDto>.Success(tokens);
    }

    public async Task<GenericResult<TokenResponseDto>> RefreshTokens()
    {
        // vremennaya zaglushka
        var tokens = new TokenResponseDto() {AccessToken = "123", RefreshToken = "456"};

        return GenericResult<TokenResponseDto>.Success(tokens);
    }

    public async Task<Result> Logout()
    {
        // vremennaya zaglushka
        return Result.Success();
    }
}