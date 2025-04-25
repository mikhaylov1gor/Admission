using Contract.Domain.Enums;

namespace DocumentService.Domain.Entities;

public class Document
{
    public ICollection<File> Files { get; set; } = new List<File>();
    public DocumentType DocumentType { get; set; }
}