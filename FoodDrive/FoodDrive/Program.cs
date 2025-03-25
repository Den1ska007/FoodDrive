using FoodDrive.Interfaces;
using FoodDrive.Models;

class Program
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllersWithViews();

        // Реєстрація репозиторіїв для ін'єкції залежностей
        services.AddScoped<IRepository<Customer>, CustomerRepository>();
    }
    
    static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Реєстрація сервісів для Dependency Injection
        builder.Services.AddScoped<IRepository<Customer>, CustomerRepository>();
        builder.Services.AddScoped<IRepository<Order>, OrderRepository>();
        builder.Services.AddScoped<IRepository<Dish>, DishRepository>();
        builder.Services.AddScoped<IRepository<Review>, ReviewRepository>();

        builder.Services.AddControllersWithViews();  // Підключення MVC

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        // Налаштування маршрутів
        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");

        app.Run();
        
    }

}
