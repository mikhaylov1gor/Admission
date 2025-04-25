namespace DocumentService.Domain.Entities;

public class EducationDocument
{
    public Guid Id { get; private set; }
    public required DateTime CreatedTime {get; set;} = DateTime.UtcNow;
    public DateTime? ModifiedTime {get; set;}
    
    public required string Name { get; set; }
}