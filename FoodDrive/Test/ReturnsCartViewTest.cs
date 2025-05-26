using FoodDrive.Controllers;
using FoodDrive.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

public class CartControllerTests
{
    private AppDbContext CreateInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: System.Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    private CartController CreateControllerWithUser(AppDbContext context, int userId)
    {
        var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Role, "Customer")
        }, "mock"));

        var httpContext = new DefaultHttpContext
        {
            User = user
        };

        return new CartController(context)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            }
        };
    }

    [Fact]
    public async Task Index_ReturnsCartView_WithItems()
    {
        // Arrange
        var dbContext = CreateInMemoryDbContext();

        var dish = new Dish
        {
            Name = "Борщ",
            Description = "Смачний український борщ",
            Price = 85,
            Stock = 10,
            Rating = 5,
            Reviews = new List<Review>()
        };
        dbContext.Dishes.Add(dish);
        dbContext.SaveChanges();

        int userId = 1;

        var cart = new Cart
        {
            UserId = userId,
            Items = new List<CartItem>
            {
                new CartItem
                {
                    DishId = dish.Id,
                    Dish = dish,
                    Quantity = 2
                }
            }
        };

        dbContext.Carts.Add(cart);
        dbContext.SaveChanges();

        var controller = CreateControllerWithUser(dbContext, userId);

        // Act
        var result = await controller.Index();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<Cart>(viewResult.Model);
        Assert.Single(model.Items);
        Assert.Equal("Борщ", model.Items.First().Dish.Name);
        Assert.Equal(2, model.Items.First().Quantity);
    }
}
