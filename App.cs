//using FastUpTime.Models;

using FastUpTime.Controllers;
using FastUpTime.Data;
using FastUpTime.Services;
using FastUpTime.Services.BackgroundServices;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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


builder.Services.AddDbContext<AppDbContext>(options =>
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
        options.Cookie.SameSite = SameSiteMode.None;
 
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


builder.Services.AddScoped<ISiteMonitoringService, SiteMonitoringService>();

builder.Services.AddHostedService<SitePingWorker>();


builder.Services.AddHttpClient();

var app = builder.Build();

// Configure the HTTP request pipeline.




app.UseHttpsRedirection();

app.UseCors("Frontend");

app.UseAuthentication();
app.UseAuthorization();



app.MapControllers();
//For Development

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

app.Run();
//dotnet ef migrations add nameOfUpdate
//dotnet ef database update //update the db