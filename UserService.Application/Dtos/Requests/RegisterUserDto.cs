using System.ComponentModel.DataAnnotations;
using Contract.Application.Validations;
using Contract.Domain.Enums;

namespace UserService.Application.Dtos.Requests;

public class RegisterUserDto
{
    [Required]
    [MinLength(1)]
    [MaxLength(100)]
    public required string FullName { get; set; }
    
    [Required]
    [EmailAddress]
    public required string Email { get; set; }
    
    [Required]
    [MinLength(6)]
    [MaxLength(25)]
    public required string Password { get; set; }
    
    [Required]
    public required DateTime Birthday { get; set; }
    
    [Required]
    public required Gender Gender { get; set; }
    
    [MinLength(1)]
    [MaxLength(100)]
    public string? Citizenship { get; set; }
    
    [PhoneNumber]
    public string? PhoneNumber { get; set; }
}