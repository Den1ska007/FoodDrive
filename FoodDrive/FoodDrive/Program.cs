// Program.cs
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using FoodDrive.Interfaces;
using FoodDrive.Models;
using System.Text.Json;
using FoodDrive.JsonConverters;
using Microsoft.EntityFrameworkCore;
using FoodDrive.Entities;
using Microsoft.AspNetCore.Identity;
class Program
{
    static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")),
            ServiceLifetime.Scoped);
        builder.Services.Configure<JsonSerializerOptions>(options =>
        {
            options.Converters.Add(new UserConverter());
        });
        builder.Services.AddScoped<UserService>();
        builder.Services.AddScoped<OrderService>();
        builder.Services.AddScoped<AuthService>();
        builder.Services.AddAuthentication("CookieAuth")
        .AddCookie("CookieAuth", options =>
        {
            options.Cookie.Name = "AuthCookie";
            options.LoginPath = "/Account/Login";  // Перенаправлення на сторінку входу
            options.AccessDeniedPath = "/Account/AccessDenied"; // Якщо немає доступу
        });


        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
            options.AddPolicy("CustomerOnly", policy => policy.RequireRole("Customer"));
        });

        builder.Services.AddControllersWithViews();

        var app = builder.Build();

        
        Directory.CreateDirectory(Path.Combine(app.Environment.ContentRootPath, "Data"));

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

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Account}/{action=Profile}/{id?}");
        app.Run();
    }
}