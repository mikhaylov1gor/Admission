namespace DocumentService.Domain.Entities;

public class EducationLevel
{
    public Guid Id { get; set; }
    
    public int ExternalId { get; set; }
    public required string Name { get; set; }
}