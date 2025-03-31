using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using FoodDrive.Interfaces;
using FoodDrive.Models;

class Program
{
    
    static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // ��������� ������ ��� Dependency Injection
        builder.Services.AddSingleton<UserRepository>();
        builder.Services.AddSingleton<DishRepository>();
        builder.Services.AddSingleton<OrderRepository>();
        builder.Services.AddSingleton<ReviewRepository>();

        builder.Services.AddControllersWithViews();  // ϳ��������� MVC

        var app = builder.Build();

        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthorization();

        // ������������ ��������
        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Admin}/{action=Index}/{id?}");

        app.Run();
        
    }

}
