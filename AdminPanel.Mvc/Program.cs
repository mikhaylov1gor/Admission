using System.Text.Json.Serialization;
using AdminPanel.Mvc.Middlewaries;
using AdminPanel.Mvc.Services.AdmissionServiceClient;
using AdminPanel.Mvc.Services.AuthService;
using AdminPanel.Mvc.Services.DictionaryServiceClient;
using AdminPanel.Mvc.Services.DocumentServiceClient;
using AdminPanel.Mvc.Services.UserServiceClient;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

// json deserialize
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

// Add services to the container.
services.AddControllersWithViews();

builder.Services.AddHttpContextAccessor();

// injections of services
services.AddScoped<IAuthService, AuthService>();
services.AddScoped<IUserServiceClient, UserServiceClient>();
services.AddScoped<IAdmissionServiceClient,AdmissionServiceClient>();
services.AddScoped<IDocumentServiceClient,DocumentServiceClient>();

// injections of external http services
services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromDays(30); 
    });

services.AddHttpClient<IUserServiceClient, UserServiceClient>(client => 
{
    client.BaseAddress = new Uri("http://localhost:5003");
});

services.AddHttpClient<IAdmissionServiceClient, AdmissionServiceClient>(client =>
{
    client.BaseAddress = new Uri("http://localhost:5004");
});

services.AddHttpClient<IDocumentServiceClient, DocumentServiceClient>(client =>
{
    client.BaseAddress = new Uri("http://localhost:5001");
});

services.AddHttpClient<IDictionaryServiceClient, DictionaryServiceClient>(client =>
{
    client.BaseAddress = new Uri("http://localhost:5000");
});

services.AddDistributedMemoryCache();
services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
});

var app = builder.Build();

// middlewaries
app.UseMiddleware<TokenRefreshMiddleware>();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}


app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); 
app.UseAuthorization();

app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();