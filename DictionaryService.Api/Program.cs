using DictionaryService.Api.Middleware;
using DictionaryService.Application.Services.DictionaryService;
using DictionaryService.Application.Services.UpdateDictionaryService;
using DictionaryService.Domain.IRepositories;
using DictionaryService.Infrastructure.Persistence;
using DictionaryService.Infrastructure.Repositories;
using DictionaryService.Infrastructure.Services.SeedDataService;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;

// dbContext injection
var connectionString = builder.Configuration.GetConnectionString("DictionaryConnection");
services.AddDbContext<DictionaryDbContext>(options =>
    options.UseNpgsql(connectionString));

// injections of repositories
services.AddScoped<IDocumentTypeRepository, DocumentTypeRepository>();
services.AddScoped<IEducationLevelRepository, EducationLevelRepository>();
services.AddScoped<IEducationProgramRepository, EducationProgramRepository>();
services.AddScoped<IFacultyRepository, FacultyRepository>();

// injections of services
services.AddHttpClient(); 
services.AddScoped<IDictionaryService, DictionaryService.Application.Services.DictionaryService.DictionaryService>();
services.AddScoped<IUpdateDictionaryService, UpdateDictionaryService>();
services.AddScoped<ISeedDataService, SeedDataService>();


builder.Services.AddHttpContextAccessor();
builder.Services.AddAuthorization();
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// injections of middlewarries
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseMiddleware<SeedDataMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
