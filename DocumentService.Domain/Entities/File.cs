namespace DocumentService.Domain.Entities;

public class File
{   
    public Guid Id { get; set; }
    public required Guid DocumentId { get; set; }
    public required Document Document { get; set; }
    public required string FileName { get; set; }
    public required string Extension { get; set; }
    public required byte[] Data { get; set; }
    public long FileSize { get; set; }
}