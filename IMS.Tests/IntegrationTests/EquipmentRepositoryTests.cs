using IMS.Application.DTO;
using IMS.Core.Model;
using IMS.Infrastructure.Repositories;
using IMS.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

namespace IMS.Tests.IntegrationTests;

public class EquipmentRepositoryTests
{
    private DbContextOptions<DataBaseContext> CreateNewContextOptions()
    {
        return new DbContextOptionsBuilder<DataBaseContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Generates a unique database name
            .Options;
    }

    // Test for GetEquipmentEntityById
    [Fact]
    public async Task GetEquipmentEntityById_ValidId_ReturnsEquipment()
    {
        // Arrange
        var options = CreateNewContextOptions();
        using (var context = new DataBaseContext(options))
        {
            var equipmentRepository = new EquipmentRepository(context);

            var equipmentList = new List<Equipment>
            {
                new Equipment
                {
                    EquipmentId = 1,
                    Name = "Microscope",
                    Model = "GS",
                    IsActive = true,
                },
                new Equipment
                {
                    EquipmentId = 2,
                    Name = "Oscilloscope",
                    Model = "GS",
                    IsActive = true,
                },
            };
            await context.equipments.AddRangeAsync(equipmentList);
            await context.SaveChangesAsync();

            // Act
            var result = equipmentRepository.GetEquipmentEntityById(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.EquipmentId);
            Assert.Equal("Microscope", result.Name);
        }
    }

    // Test for GetEquipmentDTOById
    [Fact]
    public async Task GetEquipmentDTOById_ValidId_ReturnsEquipmentDetailedDTO()
    {
        // Arrange
        var options = CreateNewContextOptions();
        using (var context = new DataBaseContext(options))
        {
            var equipmentRepository = new EquipmentRepository(context);

            var lab = new Lab
            {
                LabId = 1,
                LabName = "Lab1",
                LabCode = "123",
                IsActive = true,
            };
            var equipmentList = new List<Equipment>
            {
                new Equipment
                {
                    EquipmentId = 1,
                    Name = "Microscope",
                    Model = "GS",
                    Lab = lab,
                    IsActive = true,
                    LabId = 1,
                    Specification = "Spec1",
                    MaintenanceIntervalDays = 30,
                },
                new Equipment
                {
                    EquipmentId = 2,
                    Name = "Oscilloscope",
                    Model = "GS",
                    Lab = lab,
                    IsActive = true,
                    LabId = 2,
                    Specification = "Spec2",
                    MaintenanceIntervalDays = 60,
                },
            };
            var itemList = new List<Item>
            {
                new Item
                {
                    ItemId = 1,
                    EquipmentId = 1,
                    Status = "Available",
                    SerialNumber = "RT234trhnefs",
                    IsActive = true,
                },
                // additional items
            };
            await context.labs.AddAsync(lab);
            await context.equipments.AddRangeAsync(equipmentList);
            await context.items.AddRangeAsync(itemList);
            await context.SaveChangesAsync();

            // Act
            var result1 = equipmentRepository.GetEquipmentDTOById(1);
            var result2 = equipmentRepository.GetEquipmentDTOById(2);

            // Assert
            Assert.NotNull(result1);
            Assert.Equal(1, result1.equipmentId);
            Assert.Equal("Microscope", result1.name);
            // Additional assertions
            Assert.NotNull(result2);
        }
    }

    // Test for GetAllEquipmentDTOs
    [Fact]
    public async Task GetAllEquipmentDTOs_ValidLabId_ReturnsListOfEquipmentDTO()
    {
        // Arrange
        var options = CreateNewContextOptions();
        using (var context = new DataBaseContext(options))
        {
            var equipmentRepository = new EquipmentRepository(context);

            var labsList = new List<Lab>
            {
                new Lab
                {
                    LabId = 1,
                    LabName = "Lab1",
                    LabCode = "1234",
                    IsActive = true,
                },
                new Lab
                {
                    LabId = 2,
                    LabName = "Lab2",
                    LabCode = "1457",
                    IsActive = true,
                },
            };
            var equipmentList = new List<Equipment>
            {
                new Equipment
                {
                    EquipmentId = 1,
                    Name = "Microscope",
                    Model = "GS",
                    Lab = labsList[0],
                    IsActive = true,
                    LabId = 1,
                    Specification = "Spec1",
                    MaintenanceIntervalDays = 30,
                },
                new Equipment
                {
                    EquipmentId = 3,
                    Name = "Spectrometer",
                    Model = "GS",
                    Lab = labsList[1],
                    IsActive = true,
                    LabId = 2,
                    Specification = "Spec3",
                    MaintenanceIntervalDays = 90,
                },
            };
            await context.labs.AddRangeAsync(labsList);
            await context.equipments.AddRangeAsync(equipmentList);
            await context.SaveChangesAsync();

            // Act
            var result1 = equipmentRepository.GetAllEquipmentDTOs(1);
            var result2 = equipmentRepository.GetAllEquipmentDTOs(2);

            // Assert
            Assert.NotNull(result1);
            Assert.Single(result1);
            Assert.NotNull(result2);
            Assert.Single(result2);
        }
    }

    // Test for CheckIfEquipmentExists
    [Fact]
    public async Task CheckIfEquipmentExists_ValidNameModelLabId_ReturnsTrue()
    {
        // Arrange
        var options = CreateNewContextOptions();
        using (var context = new DataBaseContext(options))
        {
            var equipmentRepository = new EquipmentRepository(context);

            var equipmentList = new List<Equipment>
            {
                new Equipment
                {
                    EquipmentId = 1,
                    Name = "Microscope",
                    Model = "GS",
                    LabId = 1,
                    IsActive = true,
                },
                new Equipment
                {
                    EquipmentId = 2,
                    Name = "Oscilloscope",
                    Model = "GS",
                    LabId = 2,
                    IsActive = true,
                },
            };
            await context.equipments.AddRangeAsync(equipmentList);
            await context.SaveChangesAsync();

            // Act
            var result = equipmentRepository.CheckIfEquipmentExists("Microscope", "GS", 1);

            // Assert
            Assert.True(result);
        }
    }

    // Test for CreateNewEquipment
    [Fact]
    public async Task CreateNewEquipment_ValidCreateEquipmentDTO_ReturnsEquipmentDetailedDTO()
    {
        // Arrange
        var options = CreateNewContextOptions();
        using (var context = new DataBaseContext(options))
        {
            var equipmentRepository = new EquipmentRepository(context);

            var lab = new Lab
            {
                LabId = 1,
                LabName = "Lab1",
                LabCode = "123",
                IsActive = true,
            };
            await context.labs.AddAsync(lab);
            await context.SaveChangesAsync();

            var createEquipmentDTO = new CreateEquipmentDTO
            {
                name = "Microscope",
                model = "GS",
                labId = 1,
                imageURL = "imageURL",
                specification = "Spec1",
                maintenanceIntervalDays = 30,
            };

            // Act
            var result = equipmentRepository.CreateNewEquipment(createEquipmentDTO, lab);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.equipmentId);
        }
    }

    // Test for UpdateEquipment
    [Fact]
    public async Task UpdateEquipment_ValidEquipmentUpdateDTO_ReturnsEquipmentDetailedDTO()
    {
        // Arrange
        var dbContextOptions = new DbContextOptionsBuilder<DataBaseContext>()
            .UseInMemoryDatabase(databaseName: $"TestDatabase_{Guid.NewGuid()}")
            .Options;

        using (var context = new DataBaseContext(dbContextOptions))
        {
            var lab = new Lab
            {
                LabId = 1,
                LabName = "Lab1",
                LabCode = "123",
                IsActive = true,
            };

            var equipment = new Equipment
            {
                EquipmentId = 1,
                Name = "Microscope",
                Model = "GS",
                Lab = lab,
                LabId = 1,
                ImageURL = "imageURL",
                Specification = "Spec1",
                MaintenanceIntervalDays = 30,
                IsActive = true,
            };

            await context.labs.AddAsync(lab);
            await context.equipments.AddAsync(equipment);
            await context.SaveChangesAsync();

            var updateEquipmentDTO = new UpdateEquipmentDTO
            {
                name = "Microscope2",
                model = "GS2",
                imageURL = "imageURL2",
            };

            var equipmentRepository = new EquipmentRepository(context);

            // Act
            var result = equipmentRepository.UpdateEquipment(equipment, updateEquipmentDTO);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.equipmentId);
            Assert.Equal("Microscope2", result.name);
            Assert.Equal("GS2", result.model);
            Assert.Equal("imageURL2", result.imageUrl);
            Assert.Equal("Spec1", result.specification);
            Assert.Equal(30, result.maintenanceIntervalDays);
            Assert.Equal(1, result.labId);
            Assert.Equal("Lab1", result.labName);
        }
    }
}
