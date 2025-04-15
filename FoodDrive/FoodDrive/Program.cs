// Program.cs
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using FoodDrive.Interfaces;
using FoodDrive.Models;
using FoodDrive.Data;
using FoodDrive.Services;
using System.Text.Json;

class Program
{
    static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Реєстрація сервісів
        builder.Services.AddSingleton<IDataStorage<Admin>, JsonStorage<Admin>>();
        builder.Services.AddSingleton<IDataStorage<Customer>, JsonStorage<Customer>>();
        builder.Services.AddSingleton<IDataStorage<Dish>, JsonStorage<Dish>>();
        builder.Services.AddSingleton<IDataStorage<Order>, JsonStorage<Order>>();
        builder.Services.AddSingleton<IDataStorage<Review>, JsonStorage<Review>>();
        builder.Services.AddSingleton<IDataStorage<User>, JsonStorage<User>>(); // Додали цей рядок

        builder.Services.AddSingleton<IRepository<Admin>, AdminRepository>();
        builder.Services.AddSingleton<IRepository<Customer>, CustomerRepository>();
        builder.Services.AddSingleton<IRepository<Dish>, DishRepository>();
        builder.Services.AddSingleton<IRepository<Order>, OrderRepository>();
        builder.Services.AddSingleton<IRepository<Review>, ReviewRepository>();
        builder.Services.AddSingleton<IRepository<User>, UserRepository>();

        builder.Services.AddSingleton<UserService>();
        builder.Services.AddSingleton<AdminRepository>();
        builder.Services.AddSingleton<CustomerRepository>();
        builder.Services.AddSingleton<DishRepository>();
        builder.Services.AddSingleton<OrderRepository>();
        builder.Services.AddSingleton<ReviewRepository>();
        builder.Services.AddSingleton<UserRepository>(); // Додали цей рядок
        builder.Services.Configure<JsonSerializerOptions>(options =>
        {
            options.Converters.Add(new UserConverter());
        });                                        // Program.cs
        builder.Services.AddSingleton<IRepository<User>, UserRepository>();
        builder.Services.AddSingleton<AuthService>();
        builder.Services.AddAuthentication("CookieAuth")
            .AddCookie("CookieAuth", options =>
            {
                options.Cookie.Name = "AuthCookie";
                options.LoginPath = "/Account/Login";
                options.AccessDeniedPath = "/Account/AccessDenied";
            });

        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
            options.AddPolicy("CustomerOnly", policy => policy.RequireRole("Customer"));
        });
        builder.Services.AddControllersWithViews();

        var app = builder.Build();

        // Створюємо папку Data при запуску додатка
        Directory.CreateDirectory(Path.Combine(app.Environment.ContentRootPath, "Data"));

        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseRouting();
        app.UseAuthorization();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Admin}/{action=Index}/{id?}");

        app.Run();
    }
}