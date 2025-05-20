namespace DictionaryService.Domain.Entities;

public class Faculty
{
    public Guid Id { get; set; }
    public required DateTime CreateTime { get; set; }
    public required string Name { get; set; }
}