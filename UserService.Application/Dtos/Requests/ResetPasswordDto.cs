using System.ComponentModel.DataAnnotations;

namespace UserService.Application.Dtos.Requests;

public class ResetPasswordDto
{
    [Required]
    [MinLength(6)]
    [MaxLength(25)]
    public required string OldPassword { get; set; }
    
    [Required]
    [MinLength(6)]
    [MaxLength(25)]
    public required string NewPassword { get; set; }
}