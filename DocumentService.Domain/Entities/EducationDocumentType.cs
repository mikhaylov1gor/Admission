namespace DocumentService.Domain.Entities;

public class EducationDocumentType
{
    public Guid Id  {get; set;}
    
    public required string Name { get; set; }
    public EducationLevel EducationLevel { get; set; } = null!;
    
}