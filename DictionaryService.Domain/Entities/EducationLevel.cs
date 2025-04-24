namespace DictionaryService.Domain.Entities;

public class EducationLevel
{
    public Guid Id { get; private set; }
    public required string Name { get; set; }
}