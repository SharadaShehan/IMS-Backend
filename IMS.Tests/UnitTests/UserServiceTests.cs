using System.Collections.Generic;
using IMS.Application.DTO;
using IMS.Application.Interfaces;
using IMS.Application.Services;
using Moq;
using Xunit;

namespace IMS.Tests.UnitTests;

public class UserServiceTests
{
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly UserService _userService;

    public UserServiceTests()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _userService = new UserService(_mockUserRepository.Object);
    }

    [Fact]
    public void GetUserById_ExistingUser_ReturnsUser()
    {
        // Arrange
        var userId = 1;
        var userDTO = new UserDTO { userId = userId };
        _mockUserRepository.Setup(repo => repo.GetUserDTOById(userId)).Returns(userDTO);

        // Act
        var result = _userService.GetUserById(userId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(userId, result.userId);
    }

    [Fact]
    public void GetUserById_UserNotFound_ReturnsNull()
    {
        // Arrange
        var userId = 1;
        _mockUserRepository.Setup(repo => repo.GetUserDTOById(userId)).Returns((UserDTO)null);

        // Act
        var result = _userService.GetUserById(userId);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void GetAllUsers_ReturnsAllUsers()
    {
        // Arrange
        var users = new List<UserDTO>
        {
            new UserDTO { userId = 1 },
            new UserDTO { userId = 2 },
        };
        _mockUserRepository.Setup(repo => repo.GetAllUserDTOs()).Returns(users);

        // Act
        var result = _userService.GetAllUsers();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public void UpdateUserRole_UserNotFound_ReturnsErrorResponse()
    {
        // Arrange
        var userId = 1;
        var roleName = "Admin";
        _mockUserRepository
            .Setup(repo => repo.UpdateUserRole(userId, roleName))
            .Returns((UserDTO)null);

        // Act
        var result = _userService.UpdateUserRole(userId, roleName);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("User Not Found", result.message);
    }

    [Fact]
    public void UpdateUserRole_Success_ReturnsUpdatedUser()
    {
        // Arrange
        var userId = 1;
        var roleName = "Admin";
        var userDTO = new UserDTO { userId = userId, role = roleName };
        _mockUserRepository.Setup(repo => repo.UpdateUserRole(userId, roleName)).Returns(userDTO);

        // Act
        var result = _userService.UpdateUserRole(userId, roleName);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(userDTO, result.result);
    }
}
