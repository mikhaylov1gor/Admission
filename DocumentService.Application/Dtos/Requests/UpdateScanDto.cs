using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace DocumentService.Application.Dtos.Requests;

public class UpdateScanDto
{
    [Required]
    public required IFormFile NewFile { get; set; }
}