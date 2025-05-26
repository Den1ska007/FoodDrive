using FoodDrive.Entities;
using FoodDrive.Models.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

public class AccountControllerTests
{
    private static AppDbContext CreateInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()) // кожен тест — окрема БД
            .Options;

        return new AppDbContext(options);
    }

    private static AccountController CreateController(AppDbContext context, IPasswordHasher<User> passwordHasher)
    {
        var httpContext = new DefaultHttpContext();

        // Mock IAuthenticationService
        var authServiceMock = new Mock<IAuthenticationService>();
        authServiceMock
            .Setup(x => x.SignInAsync(
                It.IsAny<HttpContext>(),
                It.IsAny<string>(),
                It.IsAny<ClaimsPrincipal>(),
                It.IsAny<AuthenticationProperties>()))
            .Returns(Task.CompletedTask);

        // Mock IServiceProvider
        var serviceProviderMock = new Mock<IServiceProvider>();
        serviceProviderMock
            .Setup(sp => sp.GetService(typeof(IAuthenticationService)))
            .Returns(authServiceMock.Object);

        httpContext.RequestServices = serviceProviderMock.Object;

        var controller = new AccountController(context, passwordHasher)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            },
            TempData = new TempDataDictionary(
                httpContext,
                Mock.Of<ITempDataProvider>())
        };

        // 👇 Додаємо UrlHelper, бо інакше RedirectToAction в тесті впаде
        var urlHelperMock = new Mock<IUrlHelper>();
        urlHelperMock
            .Setup(u => u.Action(It.IsAny<UrlActionContext>()))
            .Returns("mocked-url");

        controller.Url = urlHelperMock.Object;

        return controller;
    }



    private static Admin CreateTestAdmin(string name, string password, IPasswordHasher<User> hasher)
    {
        return new Admin
        {
            Name = name,
            Role = "Admin",
            Address = "123 Admin St.",
            PasswordHash = hasher.HashPassword(null, password)
        };
    }

    [Fact]
    public async Task Login_ValidUser_ReturnsRedirect()
    {
        // Arrange
        var dbContext = CreateInMemoryDbContext();
        var passwordHasher = new PasswordHasher<User>();

        var admin = CreateTestAdmin("admin", "12345", passwordHasher);
        dbContext.Users.Add(admin);
        dbContext.SaveChanges();

        var controller = CreateController(dbContext, passwordHasher);
        var loginModel = new LoginViewModel
        {
            Username = "admin",
            Password = "12345"
        };

        // Act
        var result = await controller.Login(loginModel);

        // Assert
        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirect.ActionName);
        Assert.Equal("Admin", redirect.ControllerName); // або перевірити User.Role
    }

    [Fact]
    public async Task Login_InvalidPassword_ReturnsViewWithError()
    {
        // Arrange
        var dbContext = CreateInMemoryDbContext();
        var passwordHasher = new PasswordHasher<User>();

        var admin = CreateTestAdmin("admin", "correct_password", passwordHasher);
        dbContext.Users.Add(admin);
        dbContext.SaveChanges();

        var controller = CreateController(dbContext, passwordHasher);
        var loginModel = new LoginViewModel
        {
            Username = "admin",
            Password = "wrong_password"
        };

        // Act
        var result = await controller.Login(loginModel);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.False(controller.ModelState.IsValid);
        Assert.True(controller.ModelState.Values
            .SelectMany(v => v.Errors)
            .Any(e => e.ErrorMessage.Contains("Невірний логін")));
    }
}
