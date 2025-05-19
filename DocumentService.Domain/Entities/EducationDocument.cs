namespace DocumentService.Domain.Entities;

public class EducationDocument : Document
{
    public string Name { get; set; }
    public required DateTime CreatedTime {get; set;}
    public DateTime? ModifiedTime {get; set;}
    public Guid EducationDocumentTypeId { get; set; }
    public EducationDocumentType EducationDocumentType { get; set; } = null!;
}