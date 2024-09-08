using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using IMS.Presentation.Controllers;
using IMS.Application.DTO;
using IMS.Application.Model;
using IMS.Infrastructure.Services;
using IMS.Presentation.Services;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using Xunit.Abstractions;

namespace IMS.Tests.Controllers
{
    public class AdminControllerTests
    {
        private readonly DbContextOptions<DataBaseContext> _dbContextOptions;
        private readonly DataBaseContext _dbContext;
        private readonly AdminController _adminController;

        public AdminControllerTests(ITestOutputHelper output)
        {
            _dbContextOptions = new DbContextOptionsBuilder<DataBaseContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _dbContext = new DataBaseContext(_dbContextOptions);
            var tokenParserMock = new Mock<ITokenParser>();

            _adminController = new AdminController(_dbContext, tokenParserMock.Object);
        }

        [Fact]
        public async Task GetUsersList_ReturnsOkResult_WithListOfUsers()
        {
            // Arrange
            var users = new List<User>
            {
                new User { UserId = 1, Email = "user1@example.com", FirstName = "John", LastName = "Doe", ContactNumber = "1234567890", Role = "User", IsActive = true },
                new User { UserId = 2, Email = "user2@example.com", FirstName = "Jane", LastName = "Doe", ContactNumber = "0987654321", Role = "Admin", IsActive = true },
                new User { UserId = 3, Email = "user3@example.com", FirstName = "Kane", LastName = "Doe", ContactNumber = "0987234321", Role = "Student", IsActive = false },
            };

            await _dbContext.users.AddRangeAsync(users);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _adminController.GetUsersList();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<List<UserDTO>>(okResult.Value);
            Assert.Equal(users.Count(user => user.IsActive), returnValue.Count);
            User sampleUser = users.FirstOrDefault(u => u.IsActive = true);
            Assert.True(returnValue.Any(u => u.Email == sampleUser.Email));
        }

    }
}
