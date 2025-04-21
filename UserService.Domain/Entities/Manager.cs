namespace UserService.Domain.Entities;

public class Manager
{
    public Guid Id { get; set; }
    public User User { get; set; }
    
    public string FullName { get; set; }
}