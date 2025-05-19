namespace DocumentService.Domain.Entities;

public class File
{   
    public Guid Id { get; set; }
    public Guid DocumentId { get; set; }
    public Document Document { get; set; }
    public required string Extension { get; set; }
    public required string FileName { get; set; }
    public long FileSize { get; set; }
}