using System;
using Xunit;
using FoodDrive.Entities;

namespace FoodDrive.Tests
{
    // Тестовий похідний клас
    public class TestUser : User
    {
    }

    public class UserTests
    {
        [Fact]
        public void User_PropertyAssignment_WorksCorrectly()
        {
            // Arrange
            var user = new TestUser
            {
                Name = "John Doe",
                PasswordHash = "hashed_password",
                Address = "123 Main St",
                Role = "Admin"
            };

            // Act
            var now = DateTime.UtcNow;

            // Assert
            Assert.Equal("John Doe", user.Name);
            Assert.Equal("hashed_password", user.PasswordHash);
            Assert.Equal("123 Main St", user.Address);
            Assert.Equal("Admin", user.Role);
            Assert.True(user.CreatedAt <= now && user.CreatedAt > now.AddMinutes(-1));
        }

        [Fact]
        public void User_CreatedAt_IsInitialized()
        {
            // Arrange & Act
            var user = new TestUser();

            // Assert
            Assert.True(user.CreatedAt <= DateTime.UtcNow);
            Assert.True(user.CreatedAt > DateTime.UtcNow.AddMinutes(-1)); // created very recently
        }
    }
}
