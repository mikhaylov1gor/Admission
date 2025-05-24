using System.Text;
using System.Text.Json.Serialization;
using AdmissionService.Api.Configurations;
using AdmissionService.Api.Middleware;
using AdmissionService.Application.Services.AdmissionService;
using AdmissionService.Application.Services.DictionaryServiceClient;
using AdmissionService.Application.Services.DocumentServiceClient;
using AdmissionService.Application.Services.ProgramService;
using AdmissionService.Domain.IRepositories;
using AdmissionService.Infrastructure.Persistence;
using AdmissionService.Infrastructure.Repositories;
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

// json deserialize
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

// dbContext injection
var connectionString = builder.Configuration.GetConnectionString("AdmissionConnection");
services.AddDbContext<AdmissionDbContext>(options =>
    options.UseNpgsql(connectionString));

// injections of repositories
services.AddScoped<IStudentAdmissionRepository, StudentAdmissionRepository>();
services.AddScoped<IAdmissionProgramRepository, AdmissionProgramRepository>();
services.AddScoped<IAdmissionSettingRepository, AdmissionSettingsRepository>();

// injections of services
services.AddScoped<IAdmissionService, AdmissionService.Application.Services.AdmissionService.AdmissionService>();
services.AddScoped<IProgramService, ProgramService>();

// injections of external http services
services.AddHttpClient<IDictionaryServiceClient,DictionaryServiceClient>(client =>
{
    client.BaseAddress = new Uri("http://localhost:5000");
});

// injections of external http services
services.AddHttpClient<IDocumentServiceClient,DocumentServiceClient>(client =>
{
    client.BaseAddress = new Uri("http://localhost:5001");
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddAuthorization();
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// injections of middlewarries
app.UseMiddleware<ExceptionHandlingMiddleware>();

// models ensure creating
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AdmissionDbContext>();
    await dbContext.EnsureSettingsCreated(); 
}

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
