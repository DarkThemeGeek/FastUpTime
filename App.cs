//using FastUpTime.Models;

using FastUpTime.Controllers;
using FastUpTime.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;



var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        policy
            .WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
          
    });
  
});

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddDbContext<UserDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DbConnection"));
});

builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddServerSideBlazor();

builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.Cookie.SameSite = SameSiteMode.Lax;
 
        options.LoginPath = "/acc/auth/login";
        options.AccessDeniedPath = "/acc/auth/denied";

        options.ExpireTimeSpan = TimeSpan.FromDays(4);
        options.SlidingExpiration = true;
        
        options.Events.OnRedirectToLogin = context =>
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return Task.CompletedTask;
        };

        options.Events.OnRedirectToAccessDenied = context =>
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            return Task.CompletedTask;
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.



app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseCors("Frontend");

app.MapControllers();
//For Development

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<UserDbContext>();
    db.Database.Migrate();
}

app.Run();