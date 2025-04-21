using Contract.Domain.Enums;

namespace UserService.Application.Dtos.Responses;

public class ApplicantDto
{
    public Guid Id { get; set; }
    public required string Email { get; set; }
    
    public required string FullName { get; set; }
    public required DateTime BirthDay { get; set; }
    public required Gender Gender { get; set; }
    public string? Citizenship { get; set; }
    public string? Phone { get; set; }
}