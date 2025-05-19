namespace DocumentService.Application.Dtos.Responses;

public class FileDto
{
    public byte[] Data { get; set; }
    public string FileName { get; set; }
    public string ContentType { get; set; }
}