using IMS.Application.DTO;
using IMS.Core.Model;
using IMS.Infrastructure.Repositories;
using IMS.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

namespace IMS.Tests.IntegrationTests;

public class LabRepositoryTests
{
    private DbContextOptions<DataBaseContext> CreateNewContextOptions()
    {
        return new DbContextOptionsBuilder<DataBaseContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Generates a unique database name
            .Options;
    }

    // Test for GetLabEntityById
    [Fact]
    public void GetLabEntityById_ReturnsLabEntity()
    {
        // Arrange
        var options = CreateNewContextOptions();
        using (var context = new DataBaseContext(options))
        {
            var repository = new LabRepository(context);

            context.labs.Add(
                new Lab
                {
                    LabId = 1,
                    LabName = "Lab1",
                    LabCode = "ICE356",
                    ImageURL = "url",
                    IsActive = true,
                }
            );
            context.SaveChanges();

            // Act
            var lab = repository.GetLabEntityById(1);

            // Assert
            Assert.NotNull(lab);
            Assert.Equal("Lab1", lab.LabName);
            Assert.Equal("ICE356", lab.LabCode);
            Assert.Equal("url", lab.ImageURL);
            Assert.True(lab.IsActive);
        }
    }

    // Test for GetLabDTOById
    [Fact]
    public void GetLabDTOById_ReturnsLabDTO()
    {
        // Arrange
        var options = CreateNewContextOptions();
        using (var context = new DataBaseContext(options))
        {
            var repository = new LabRepository(context);

            context.labs.Add(
                new Lab
                {
                    LabId = 1,
                    LabName = "Lab1",
                    LabCode = "ICE356",
                    ImageURL = "url",
                    IsActive = true,
                }
            );
            context.SaveChanges();

            // Act
            var lab = repository.GetLabDTOById(1);

            // Assert
            Assert.NotNull(lab);
            Assert.Equal("Lab1", lab.labName);
            Assert.Equal("ICE356", lab.labCode);
            Assert.Equal("url", lab.imageUrl);
        }
    }

    [Fact]
    public void GetLabDTOById_InvalidLabId_ReturnsNull()
    {
        // Arrange
        var options = CreateNewContextOptions();
        using (var context = new DataBaseContext(options))
        {
            var repository = new LabRepository(context);

            context.labs.Add(
                new Lab
                {
                    LabId = 1,
                    LabName = "Lab1",
                    LabCode = "ICE356",
                    ImageURL = "url",
                    IsActive = true,
                }
            );
            context.SaveChanges();

            // Act
            var lab = repository.GetLabDTOById(2);

            // Assert
            Assert.Null(lab);
        }
    }

    // Test for GetAllLabDTOs
    [Fact]
    public void GetAllLabDTOs_ReturnsListOfLabDTOs()
    {
        // Arrange
        var options = CreateNewContextOptions();
        using (var context = new DataBaseContext(options))
        {
            var repository = new LabRepository(context);

            context.labs.Add(
                new Lab
                {
                    LabId = 1,
                    LabName = "Lab1",
                    LabCode = "ICE356",
                    ImageURL = "url",
                    IsActive = true,
                }
            );
            context.labs.Add(
                new Lab
                {
                    LabId = 2,
                    LabName = "Lab2",
                    LabCode = "ICE357",
                    ImageURL = "url",
                    IsActive = true,
                }
            );
            context.labs.Add(
                new Lab
                {
                    LabId = 3,
                    LabName = "Lab3",
                    LabCode = "JUT357",
                    ImageURL = "url",
                    IsActive = false,
                }
            );
            context.SaveChanges();

            // Act
            var labs = repository.GetAllLabDTOs();

            // Assert
            Assert.NotNull(labs);
            Assert.Equal(2, labs.Count);
            Assert.Equal("Lab1", labs[0].labName);
            Assert.Equal("Lab2", labs[1].labName);
        }
    }

    // Test for CheckIfLabExists
    [Fact]
    public void CheckIfLabExists_ValidLabNameLabCode_ReturnsTrue()
    {
        // Arrange
        var options = CreateNewContextOptions();
        using (var context = new DataBaseContext(options))
        {
            var repository = new LabRepository(context);

            context.labs.Add(
                new Lab
                {
                    LabId = 1,
                    LabName = "Lab1",
                    LabCode = "ICE356",
                    IsActive = true,
                }
            );
            context.SaveChanges();

            // Act
            var result = repository.CheckIfLabExists("Lab1", "ICE356");

            // Assert
            Assert.True(result);
        }
    }

    [Fact]
    public void CheckIfLabExists_InvalidLabNameLabCode_ReturnsFalse()
    {
        // Arrange
        var options = CreateNewContextOptions();
        using (var context = new DataBaseContext(options))
        {
            var repository = new LabRepository(context);

            context.labs.Add(
                new Lab
                {
                    LabId = 1,
                    LabName = "Lab1",
                    LabCode = "ICE356",
                    IsActive = true,
                }
            );
            context.SaveChanges();

            // Act
            var result = repository.CheckIfLabExists("Lab2", "ICE357");

            // Assert
            Assert.False(result);
        }
    }

    // Test for CreateNewLab
    [Fact]
    public void CreateNewLab_ValidCreateLabDTO_ReturnsLabDTO()
    {
        // Arrange
        var options = CreateNewContextOptions();
        using (var context = new DataBaseContext(options))
        {
            var repository = new LabRepository(context);

            var createLabDTO = new CreateLabDTO
            {
                labName = "Lab1",
                labCode = "ICE356",
                imageURL = "url",
            };

            // Act
            var lab = repository.CreateNewLab(createLabDTO);

            // Assert
            Assert.NotNull(lab);
            Assert.Equal("Lab1", lab.labName);
            Assert.Equal("ICE356", lab.labCode);
            Assert.Equal("url", lab.imageUrl);
        }
    }

    // Test for UpdateLab
    [Fact]
    public void UpdateLab_ValidUpdateLabDTO_ReturnsLabDTO()
    {
        // Arrange
        var options = CreateNewContextOptions();
        using (var context = new DataBaseContext(options))
        {
            var repository = new LabRepository(context);

            context.labs.Add(
                new Lab
                {
                    LabId = 1,
                    LabName = "Lab1",
                    LabCode = "ICE356",
                    ImageURL = "url",
                    IsActive = true,
                }
            );
            context.SaveChanges();

            var updateLabDTO = new UpdateLabDTO
            {
                labName = "Lab2",
                labCode = "ICE357",
                imageURL = "url2",
            };

            // Act
            var lab = repository.GetLabEntityById(1);
            var updatedLab = repository.UpdateLab(lab, updateLabDTO);

            // Assert
            Assert.NotNull(updatedLab);
            Assert.Equal("Lab2", updatedLab.labName);
            Assert.Equal("ICE357", updatedLab.labCode);
            Assert.Equal("url2", updatedLab.imageUrl);
        }
    }
}
