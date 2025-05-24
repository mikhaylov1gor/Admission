using System.Text;
using DocumentService.Api.Middleware;
using DocumentService.Application.Services.AdmissionServiceClient;
using DocumentService.Application.Services.DictionaryServiceClient;
using DocumentService.Application.Services.DocumentService;
using DocumentService.Application.Services.ScanService;
using DocumentService.Domain.IRepositories;
using DocumentService.Infrastructure.Configurations;
using DocumentService.Infrastructure.Persistence;
using DocumentService.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;

// Jwt configuration
var jwtSettings = builder.Configuration.GetSection("Jwt");
services.Configure<JwtSettings>(jwtSettings);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]))
        };
    });

builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter JWT token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

// dbContext injection
var connectionString = builder.Configuration.GetConnectionString("DocumentConnection");
services.AddDbContext<DocumentDbContext>(options =>
    options.UseNpgsql(connectionString));

// injections of repositories
services.AddScoped<IDocumentRepository, DocumentRepository>();
services.AddScoped<IEducationDocumentRepository, EducationDocumentRepository>();
services.AddScoped<IPassportRepository, PassportRepository>();
services.AddScoped<IEducationDocumentTypeRepository, EducationDocumentTypeRepository>();

// injections of services
services.AddScoped<IDocumentService, DocumentService.Application.Services.DocumentService.DocumentService>();
services.AddScoped<IScanService, ScanService>();

builder.Services.AddHttpContextAccessor();
builder.Services.AddAuthorization();
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// injections of external http services
services.AddHttpClient<IDictionaryServiceClient,DictionaryServiceClient>(client =>
{
    client.BaseAddress = new Uri("http://localhost:5000");
});

services.AddHttpClient<IAdmissionServiceClient,AdmissionServiceClient>(client =>
{
    client.BaseAddress = new Uri("http://localhost:5004");
});


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
