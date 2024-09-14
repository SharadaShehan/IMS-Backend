using IMS.Application.DTO;
using IMS.Core.Model;
using IMS.Infrastructure.Repositories;
using IMS.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

namespace IMS.Tests.IntegrationTests;

public class UserRepositoryTests
{
    private DbContextOptions<DataBaseContext> CreateNewContextOptions()
    {
        return new DbContextOptionsBuilder<DataBaseContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Generates a unique database name
            .Options;
    }

    // Test for GetUserEntityById
    [Fact]
    public void GetUserEntityById_ReturnsUserEntity()
    {
        // Arrange
        var options = CreateNewContextOptions();
        using (var context = new DataBaseContext(options))
        {
            var repository = new UserRepository(context);

            context.users.Add(
                new User
                {
                    UserId = 1,
                    Email = "me@email.com",
                    FirstName = "John",
                    LastName = "Doe",
                    ContactNumber = "123453647",
                    Role = "Admin",
                    IsActive = true,
                }
            );
            context.SaveChanges();

            // Act
            var user = repository.GetUserEntityById(1);

            // Assert
            Assert.NotNull(user);
            Assert.Equal("me@email.com", user.Email);
            Assert.True(user.IsActive);
        }
    }

    // Test for GetUserEntityById - Fail (User Not Found)
    [Fact]
    public void GetUserEntityById_ReturnsNull_WhenUserDoesNotExist()
    {
        // Arrange
        var options = CreateNewContextOptions();
        using (var context = new DataBaseContext(options))
        {
            var repository = new UserRepository(context);

            // Act
            var user = repository.GetUserEntityById(1);

            // Assert
            Assert.Null(user);
        }
    }

    // Test for GetUserDTOById - Success
    [Fact]
    public void GetUserDTOById_ReturnsUserDTO()
    {
        // Arrange
        var options = CreateNewContextOptions();
        using (var context = new DataBaseContext(options))
        {
            var repository = new UserRepository(context);

            context.users.Add(
                new User
                {
                    UserId = 1,
                    Email = "me@email.com",
                    FirstName = "John",
                    LastName = "Doe",
                    ContactNumber = "123453647",
                    Role = "Admin",
                    IsActive = true,
                }
            );
            context.SaveChanges();

            // Act
            var userDTO = repository.GetUserDTOById(1);

            // Assert
            Assert.NotNull(userDTO);
            Assert.Equal("me@email.com", userDTO.email);
            Assert.Equal("John", userDTO.firstName);
            Assert.Equal("Doe", userDTO.lastName);
        }
    }

    // Test for GetUserDTOById - Fail (User Not Found)
    [Fact]
    public void GetUserDTOById_ReturnsNull_WhenUserDoesNotExist()
    {
        // Arrange
        var options = CreateNewContextOptions();
        using (var context = new DataBaseContext(options))
        {
            var repository = new UserRepository(context);

            // Act
            var userDTO = repository.GetUserDTOById(1);

            // Assert
            Assert.Null(userDTO);
        }
    }

    // Test for GetAllUserDTOs - Success
    [Fact]
    public void GetAllUserDTOs_ReturnsListOfActiveUsers()
    {
        // Arrange
        var options = CreateNewContextOptions();
        using (var context = new DataBaseContext(options))
        {
            var repository = new UserRepository(context);

            context.users.AddRange(
                new User
                {
                    UserId = 1,
                    Email = "user1@email.com",
                    FirstName = "User1",
                    LastName = "Lname",
                    ContactNumber = "123453647",
                    Role = "Admin",
                    IsActive = true,
                },
                new User
                {
                    UserId = 2,
                    Email = "user2@email.com",
                    FirstName = "User2",
                    LastName = "Lname",
                    ContactNumber = "123453647",
                    Role = "Student",
                    IsActive = true,
                },
                new User
                {
                    UserId = 3,
                    Email = "user3@email.com",
                    FirstName = "User3",
                    LastName = "Lname",
                    ContactNumber = "123453647",
                    Role = "Student",
                    IsActive = false,
                }
            );
            context.SaveChanges();

            // Act
            var users = repository.GetAllUserDTOs();

            // Assert
            Assert.Equal(2, users.Count); // Only active users should be returned
            Assert.Contains(users, u => u.email == "user1@email.com");
            Assert.Contains(users, u => u.email == "user2@email.com");
        }
    }

    // Test for UpdateUserRole - Success
    [Fact]
    public void UpdateUserRole_UpdatesRoleSuccessfully()
    {
        // Arrange
        var options = CreateNewContextOptions();
        using (var context = new DataBaseContext(options))
        {
            var repository = new UserRepository(context);

            context.users.Add(
                new User
                {
                    UserId = 1,
                    Email = "me@email.com",
                    FirstName = "John",
                    LastName = "Doe",
                    ContactNumber = "123453647",
                    Role = "Clerk",
                    IsActive = true,
                }
            );
            context.SaveChanges();

            // Act
            var updatedUser = repository.UpdateUserRole(1, "Admin");

            // Assert
            Assert.NotNull(updatedUser);
            Assert.Equal("Admin", updatedUser.role);
        }
    }

    // Test for UpdateUserRole - Fail (User Not Found)
    [Fact]
    public void UpdateUserRole_ReturnsNull_WhenUserDoesNotExist()
    {
        // Arrange
        var options = CreateNewContextOptions();
        using (var context = new DataBaseContext(options))
        {
            var repository = new UserRepository(context);

            // Act
            var updatedUser = repository.UpdateUserRole(1, "Admin");

            // Assert
            Assert.Null(updatedUser);
        }
    }

    // Test for UpdateUserRole - Fail (Inactive User)
    [Fact]
    public void UpdateUserRole_ReturnsNull_WhenUserIsInactive()
    {
        // Arrange
        var options = CreateNewContextOptions();
        using (var context = new DataBaseContext(options))
        {
            var repository = new UserRepository(context);

            context.users.Add(
                new User
                {
                    UserId = 1,
                    Email = "me@email.com",
                    FirstName = "John",
                    LastName = "Doe",
                    ContactNumber = "123453647",
                    Role = "Clerk",
                    IsActive = false,
                }
            );
            context.SaveChanges();

            // Act
            var updatedUser = repository.UpdateUserRole(1, "Admin");

            // Assert
            Assert.Null(updatedUser);
        }
    }
}
