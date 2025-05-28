using System.Text;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using UserService.Api.Context.UserContext;
using UserService.Api.Middleware;
using UserService.Application.Interfaces;
using UserService.Application.Services.ApplicantService;
using UserService.Application.Services.AuthService;
using UserService.Domain.IRepositories;
using UserService.Infrastructure.Configurations;
using UserService.Infrastructure.Persistence;
using UserService.Infrastructure.Repositories;
using UserService.Infrastructure.Security;

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
var connectionString = builder.Configuration.GetConnectionString("UserConnection");
services.AddDbContext<UserDbContext>(options =>
    options.UseNpgsql(connectionString));

// injections of repositories
services.AddScoped<IUserRepository, UserRepository>();
services.AddScoped<IApplicantRepository, ApplicantRepository>();
services.AddScoped<IManagerRepository, ManagerRepository>();
services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

// injections of services
services.AddScoped<IApplicantService, ApplicantService>();
services.AddScoped<IAuthService, AuthService>();
services.AddScoped<IPasswordHasherService, PasswordHasherService>();
services.AddScoped<ITokenService, TokenService>();
services.AddScoped<IUserContextService, UserContextService>();

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
    var dbContext = scope.ServiceProvider.GetRequiredService<UserDbContext>();
    await dbContext.EnsureAdminCreated(); 
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
