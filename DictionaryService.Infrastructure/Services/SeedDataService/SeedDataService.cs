using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using DictionaryService.Application.Dtos.Responses;
using DictionaryService.Domain.Entities;
using DictionaryService.Infrastructure.Models.ExternalModels;
using DictionaryService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DictionaryService.Infrastructure.Services.SeedDataService;

public class SeedDataService : ISeedDataService
{
    private readonly DictionaryDbContext _context;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<SeedDataService> _logger;
    
    const string Login = "student";
    const string Password = "ny6gQnyn4ecbBrP9l1Fz";
    
    public SeedDataService(DictionaryDbContext dbContext, IHttpClientFactory httpClientFactory, ILogger<SeedDataService> logger)
    {
        _context = dbContext;
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public async Task SeedEducationLevelsAsync()
    {
        if (_context.EducationLevels.Any()) return;

        var externalDtos = await GetExternalData<List<ExternalEducationLevelDto>>(
            "https://1c-mockup.kreosoft.space/api/dictionary/education_levels");

        foreach (var ext in externalDtos)
        {
            await _context.EducationLevels.AddAsync(new EducationLevel
            {
                Id = Guid.NewGuid(),
                Name = ext.name
            });
        }
        await _context.SaveChangesAsync();
    }
    public async Task SeedDocumentTypesAsync()
    {
        if (_context.EducationDocumentTypes.Any()) return;

        var externalTypes = await GetExternalData<List<ExternalEducationDocumentType>>(
            "https://1c-mockup.kreosoft.space/api/dictionary/document_types");

        foreach (var dto in externalTypes)
        {
            var educationLevel = await _context.EducationLevels
                .FirstOrDefaultAsync(el => el.Name == dto.educationLevel.name);

            if (educationLevel == null)
            {
                _logger.LogWarning("EducationLevel not found: {Name}", dto.educationLevel.name);
                continue;
            }

            var nextLevels = dto.nextEducationLevels?
                .Join(_context.EducationLevels,
                    ext => ext.name,
                    el => el.Name,
                    (ext, el) => el)
                .ToList() ?? new List<EducationLevel>();

            await _context.EducationDocumentTypes.AddAsync(new EducationDocumentType
            {
                Id = dto.id,
                CreateTime = dto.createTime.ToUniversalTime(),
                Name = dto.name,
                EducationLevel = educationLevel,
                NextEducationLevels = nextLevels
            });
        }
        await _context.SaveChangesAsync();
    }

    public async Task SeedFacultiesAsync()
    {
        if (_context.Faculties.Any()) return;

        var externalDtos = await GetExternalData<List<ExternalFaculty>>(
            "https://1c-mockup.kreosoft.space/api/dictionary/faculties");

        foreach (var ext in externalDtos)
        {
            await _context.Faculties.AddAsync(new Faculty
            {
                Id = ext.id,
                Name = ext.name,
                CreateTime = ext.createTime.ToUniversalTime()
            });
        }

        await _context.SaveChangesAsync();
    }

    public async Task SeedProgramsAsync()
    {
        if (_context.EducationPrograms.Any()) return;
        var url = "https://1c-mockup.kreosoft.space/api/dictionary/programs?page=1&size=1000000";
        var response = await GetExternalData<ExternalProgramsResponse>(url);

        foreach (var ext in response.programs)
        {
            var faculty = await _context.Faculties.FindAsync(ext.faculty.id);
            if (faculty == null) continue;

            var educationLevel = await _context.EducationLevels
                .FirstOrDefaultAsync(el => el.Name == ext.educationLevel.name);
            if (educationLevel == null) continue;

            await _context.EducationPrograms.AddAsync(new EducationProgram
            {
                Id = ext.id,
                Name = ext.name,
                CreateTime = ext.createTime.ToUniversalTime(),
                Code = ext.code,
                Language = ext.language,
                EducationForm = ext.educationForm,
                Faculty = faculty,
                EducationLevel = educationLevel
            });
        }
        await _context.SaveChangesAsync();
    }
    
    private async Task<T> GetExternalData<T>(string url)
    {
        var client = _httpClientFactory.CreateClient();
        var authToken = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{Login}:{Password}"));
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authToken);

        var response = await client.GetAsync(url);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<T>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
    }
}