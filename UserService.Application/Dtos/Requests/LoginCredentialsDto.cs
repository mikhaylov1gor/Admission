using System.ComponentModel.DataAnnotations;

namespace UserService.Application.Dtos.Requests;

public class LoginCredentialsDto
{
    [Required]
    [EmailAddress]
    public required string Email {get; set;}
    
    [Required]
    [MinLength(6)]
    [MaxLength(25)]
    public required string Password {get; set;}
}