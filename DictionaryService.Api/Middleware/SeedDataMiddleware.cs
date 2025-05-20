using DictionaryService.Infrastructure.Services.SeedDataService;

namespace DictionaryService.Api.Middleware;

public class SeedDataMiddleware
{
    private readonly RequestDelegate _next;

    public SeedDataMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, ISeedDataService _seedDataService)
    {
        await _seedDataService.SeedEducationLevelsAsync();
        await _seedDataService.SeedDocumentTypesAsync();
        await _seedDataService.SeedFacultiesAsync();
        await _seedDataService.SeedProgramsAsync();
        
        await _next(context);
    }
}