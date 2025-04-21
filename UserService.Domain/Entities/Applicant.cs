using Contract.Domain.Enums;

namespace UserService.Domain.Entities;

public class Applicant
{
    public Guid Id { get; set; }
    public User User { get; set; }
    
    public required string FullName { get; set; }
    public DateTime BirthDay { get; set; }
    public Gender Gender { get; set; }
    public string? Citizenship { get; set; }
    public string? PhoneNumber { get; set; }
}