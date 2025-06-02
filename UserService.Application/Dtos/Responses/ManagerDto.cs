using Contract.Domain.Enums;

namespace UserService.Application.Dtos.Responses;

public class ManagerDto
{
    public Guid Id { get; set; }
    
    public string Email { get; set; }
    public string FullName { get; set; }
    public Role Role { get; set; }
}