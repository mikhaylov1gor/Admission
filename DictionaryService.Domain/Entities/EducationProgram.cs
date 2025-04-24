namespace DictionaryService.Domain.Entities;

public class EducationProgram
{
    public Guid Id { get; private set; }
    public required DateTime CreateTime { get; set; }
    public required string Name { get; set; }
    public string? Code {get; set;}
    public required string Language { get; set; }
    public required string EducationForm { get; set; }
    
    public required Faculty Faculty { get; set; }
    public required EducationLevel EducationLevel { get; set; }
}