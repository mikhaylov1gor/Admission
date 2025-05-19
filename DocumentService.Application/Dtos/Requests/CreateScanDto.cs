using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace DocumentService.Application.Dtos.Requests;

public class CreateScanDto
{
    [Required]
    public required IFormFile File { get; set; }
}