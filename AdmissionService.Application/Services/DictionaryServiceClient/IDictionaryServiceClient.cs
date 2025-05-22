using System.Text.Json;
using AdmissionService.Application.Dtos.Responses;
using Microsoft.Extensions.Logging;

namespace AdmissionService.Application.Services.DictionaryServiceClient;

public interface IDictionaryServiceClient
{
    Task<EducationProgramDto?> GetEducationProgramById(Guid programId);
    Task<EducationLevelDto?> GetEducationLevelById(Guid educationLevelId);
}