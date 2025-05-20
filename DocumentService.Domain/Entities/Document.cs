using Contract.Domain.Enums;

namespace DocumentService.Domain.Entities;

public class Document
{
    public Guid Id { get; set; }
    public required Guid ApplicantId { get; set; }
    public required DateTime CreatedTime {get; set;}
    public DateTime? ModifiedTime {get; set;}
    
    public ICollection<File> Files { get; set; } = new List<File>();
    public required DocumentType DocumentType { get; set; }
}