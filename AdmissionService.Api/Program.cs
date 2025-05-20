using AdmissionService.Api.Middleware;
using AdmissionService.Application.Services.AdmissionService;
using AdmissionService.Domain.IRepositories;
using AdmissionService.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;
// Jwt configuration

// dbContext injection

// injections of repositories
services.AddScoped<IStudentAdmissionRepository, StudentAdmissionRepository>();
services.AddScoped<IAdmissionProgramRepository, AdmissionProgramRepository>();

// injections of services
services.AddScoped<IAdmissionService, AdmissionService.Application.Services.AdmissionService.AdmissionService>();

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
