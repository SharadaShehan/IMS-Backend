using Microsoft.AspNetCore.Mvc;
using Moq;
using IMS.Presentation.Controllers;
using IMS.ApplicationCore.DTO;
using IMS.Presentation.Services;
using IMS.ApplicationCore.Model;
using IMS.Infrastructure.Services;
using IMS.Tests.Utilities;
using Microsoft.EntityFrameworkCore;

namespace IMS.Tests.Controllers
{
    public class UserControllerTests
    {
        protected readonly Mock<DataBaseContext> _dbContextMock;
        protected readonly Mock<ITokenParser> _tokenParserMock;
        protected readonly UserController _controller;

        public UserControllerTests()
        {
            var options = new DbContextOptionsBuilder<DataBaseContext>().Options;
            _dbContextMock = new Mock<DataBaseContext>(options);
            _tokenParserMock = new Mock<ITokenParser>();
            _controller = new UserController(_dbContextMock.Object, _tokenParserMock.Object);
        }

        [Fact]
        public async Task GetUserRole_ValidToken_ReturnsUserRole()
        {
            // Arrange
            var inputAuthHeader = "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJlbWFpbCI6IkxlZUNIQHVvZS51cyIsImZpcnN0TmFtZSI6IkhlbGVuIiwibGFzdE5hbWUiOiJMZWUiLCJjb250YWN0TnVtYmVyIjoiKzQ0NzY1NDMyMTkwIiwiY2xpZW50X2lkIjoiZ3JvdXAyMi1jbGllbnQtaWQiLCJpYXQiOjE3MjUxOTI2MTEsImV4cCI6MTcyNTE5MzUxMX0.UJLH2flZtvrwia61KGOmRzV2IkhFGhYmkhlzPJgE2Q8";
            var expectedRole = "Student";
            var user = new UserDTO { Role = expectedRole };
            _tokenParserMock.Setup(tp => tp.getUser(It.Is<string>(s => s == inputAuthHeader))).ReturnsAsync(user);
            // Mock the HttpContext
            HttpContextHelper.MockHttpContext(_controller, inputAuthHeader);

            // Act
            var result = await _controller.GetUserRole();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var userRoleDto = Assert.IsType<UserRoleDTO>(okResult.Value);
            Assert.Equal(expectedRole, userRoleDto.RoleName);
        }

        // Other tests...
    }
}
