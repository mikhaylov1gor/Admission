using DocumentService.Api.Middleware;
using DocumentService.Application.Services.DocumentService;
using DocumentService.Application.Services.ScanService;
using DocumentService.Domain.IRepositories;
using DocumentService.Infrastructure.Persistence;
using DocumentService.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);


var services = builder.Services;

// dbContext injection
var connectionString = builder.Configuration.GetConnectionString("DocumentConnection");
services.AddDbContext<DocumentDbContext>(options =>
    options.UseNpgsql(connectionString));

// injections of repositories
services.AddScoped<IDocumentRepository, DocumentRepository>();
services.AddScoped<IEducationDocumentRepository, EducationDocumentRepository>();
services.AddScoped<IPassportRepository, PassportRepository>();

// injections of services
services.AddScoped<IDocumentService, DocumentService.Application.Services.DocumentService.DocumentService>();
services.AddScoped<IScanService, ScanService>();

builder.Services.AddHttpContextAccessor();
builder.Services.AddAuthorization();
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// injections of middlewarries
app.UseMiddleware<ExceptionHandlingMiddleware>();

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
