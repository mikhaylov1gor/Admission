using Microsoft.EntityFrameworkCore;
using UserService.Application.Interfaces;
using UserService.Application.Services.ApplicantService;
using UserService.Application.Services.AuthService;
using UserService.Domain.IRepositories;
using UserService.Infrastructure.Persistence;
using UserService.Infrastructure.Repositories;
using UserService.Infrastructure.Security;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var services = builder.Services;

// регистрация dbContext
var connectionString = builder.Configuration.GetConnectionString("UserConnection");
services.AddDbContext<UserDbContext>(options =>
    options.UseNpgsql(connectionString));

// регистрация репозиториев
services.AddScoped<IUserRepository, UserRepository>();
services.AddScoped<IApplicantRepository, ApplicantRepository>();
services.AddScoped<IManagerRepository, ManagerRepository>();

// регистрация сервисов
services.AddScoped<IApplicantService, ApplicantService>();
services.AddScoped<IAuthService, AuthService>();
services.AddScoped<IPasswordHasherService, PasswordHasherService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.Run();
