using DocumentService.Application.Services.DocumentService;

var builder = WebApplication.CreateBuilder(args);


var services = builder.Services;

// dbContext injection

// injections of repositories

// injections of services
services.AddScoped<IDocumentService, DocumentService.Application.Services.DocumentService.DocumentService>();

builder.Services.AddHttpContextAccessor();
builder.Services.AddAuthorization();
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// injections of middlewarries

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
