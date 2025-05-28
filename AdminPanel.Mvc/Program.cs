using AdminPanel.Mvc.Middlewaries;
using AdminPanel.Mvc.Services.AuthService;
using AdminPanel.Mvc.Services.UserServiceClient;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

// Add services to the container.
services.AddControllersWithViews();

builder.Services.AddHttpContextAccessor();

// injections of services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserServiceClient, UserServiceClient>();

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