using Contract.Domain.Enums;

namespace UserService.Domain.Entities;

public class User
{
    public Guid Id { get; set; }
    
    public required string Email { get; set; }
    public required string HashedPassword { get; set; }
    public Role Role { get; set; }
}