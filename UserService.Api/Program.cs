using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
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
services.AddSingleton<ITokenService, TokenService>();

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

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.Run();
