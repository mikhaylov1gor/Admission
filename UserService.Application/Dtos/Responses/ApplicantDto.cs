using Contract.Domain.Enums;

namespace UserService.Application.Dtos.Responses;

public class ApplicantDto
{
    public Guid Id { get; set; }
    public string Email { get; set; }
    
    public string FullName { get; set; }
    public DateTime BirthDay { get; set; }
    public Gender Gender { get; set; }
    public string? Citizenship { get; set; }
    public string? PhoneNumber { get; set; }
}