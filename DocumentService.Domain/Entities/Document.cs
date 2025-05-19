using Contract.Domain.Enums;

namespace DocumentService.Domain.Entities;

public class Document
{
    public Guid Id { get; set; }
    public required Guid ApplicantId { get; set; }
    
    public ICollection<File> Files { get; set; } = new List<File>();
    public required DocumentType DocumentType { get; set; }
}