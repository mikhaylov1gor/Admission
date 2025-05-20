namespace DocumentService.Application.Dtos.Responses;

public class DownloadFileDto
{
    public byte[] Data { get; set; } = null!;
    public string FileName { get; set; } = null!;
    public string ContentType { get; set; } = null!;
}