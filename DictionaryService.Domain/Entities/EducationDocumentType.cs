namespace DictionaryService.Domain.Entities;

public class EducationDocumentType
{
    public Guid Id { get; private set; }
    public required DateTime createTime {get; set;}
    public required string Name {get; set;}
    public required EducationLevel EducationLevel {get; set;}
    public List<EducationLevel>? NextEducationLevel {get; set;}
}