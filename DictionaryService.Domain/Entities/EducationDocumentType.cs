namespace DictionaryService.Domain.Entities;

public class EducationDocumentType
{
    public Guid Id { get; set; }
    public required DateTime CreateTime {get; set;}
    public required string Name {get; set;}
    public required EducationLevel EducationLevel {get; set;}
    public List<EducationLevel>? NextEducationLevels {get; set;}
}