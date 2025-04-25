namespace DocumentService.Domain.Entities;

public class File
{
    public Guid Id { get; set; }
    public required string Extension { get; set; }
    public required string Name { get; set; }
    public required long Size { get; set; }
}