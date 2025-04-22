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
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IPasswordHasherService _passwordHasherService;
    private readonly ITokenService _tokenService;

    public AuthService(
        IUserRepository userRepository,
        IApplicantRepository applicantRepository,
        IRefreshTokenRepository refreshTokenRepository,
        IPasswordHasherService passwordHasherService,
        ITokenService tokenService)
    {
        _userRepository = userRepository;
        _applicantRepository = applicantRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _passwordHasherService = passwordHasherService;
        _tokenService = tokenService;
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
        
        var tokens = await _tokenService.GenerateTokens(newUser);

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
        
        var tokens = await _tokenService.GenerateTokens(user);

        return GenericResult<TokenResponseDto>.Success(tokens);
    }

    public async Task<GenericResult<TokenResponseDto>> RefreshTokens(string refreshToken)
    {
        var tokens = await _tokenService.RefreshToken(refreshToken);

        return GenericResult<TokenResponseDto>.Success(tokens);
    }

    public async Task<Result> Logout(Guid userId)
    {
        var applicantDb = await _applicantRepository.GetByUserIdAsync(userId);

        if (applicantDb == null)
        {
            throw new NotFoundException("Applicant not found.");
        }
        
        var refreshTokens = await _refreshTokenRepository.GetAllByUserIdAsync(userId);

        if (refreshTokens.Count == 0)
        {
            throw new NotFoundException("Refresh tokens not found.");
        }
        
        await _tokenService.DeleteTokens(refreshTokens);

        return Result.Success();
    }
}