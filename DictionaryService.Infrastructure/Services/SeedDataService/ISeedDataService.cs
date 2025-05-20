namespace DictionaryService.Infrastructure.Services.SeedDataService;

public interface ISeedDataService
{
    public Task SeedEducationLevelsAsync();
    Task SeedDocumentTypesAsync();
    Task SeedFacultiesAsync();
    Task SeedProgramsAsync();
}