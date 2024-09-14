using IMS.Application.DTO;
using IMS.Core.Model;
using IMS.Infrastructure.Repositories;
using IMS.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

namespace IMS.Tests.IntegrationTests;

public class ItemRepositoryTests
{
    private DbContextOptions<DataBaseContext> CreateNewContextOptions()
    {
        return new DbContextOptionsBuilder<DataBaseContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Generates a unique database name
            .Options;
    }

    // Test for GetItemEntityById
    [Fact]
    public void GetItemEntityById_ReturnsItem()
    {
        // Arrange
        var options = CreateNewContextOptions();
        using (var context = new DataBaseContext(options))
        {
            context.labs.Add(
                new Lab
                {
                    LabId = 1,
                    LabName = "Lab1",
                    LabCode = "5674",
                    IsActive = true,
                }
            );
            context.equipments.Add(
                new Equipment
                {
                    EquipmentId = 1,
                    Name = "Equipment1",
                    Model = "Model1",
                    ImageURL = "url",
                    LabId = 1,
                    IsActive = true,
                }
            );
            context.items.Add(
                new Item
                {
                    ItemId = 1,
                    EquipmentId = 1,
                    Status = "Available",
                    SerialNumber = "RT234trhnefs",
                    IsActive = true,
                }
            );
            context.SaveChanges();

            // Act
            var repository = new ItemRepository(context);
            var item = repository.GetItemEntityById(1);

            // Assert
            Assert.NotNull(item);
            Assert.Equal(1, item.ItemId);
            Assert.Equal(1, item.Equipment.EquipmentId);
        }
    }

    // Test for GetItemDTOById
    [Fact]
    public void GetItemDTOById_ReturnsItemDTO()
    {
        // Arrange
        var options = CreateNewContextOptions();
        using (var context = new DataBaseContext(options))
        {
            context.items.Add(
                new Item
                {
                    ItemId = 1,
                    EquipmentId = 1,
                    Status = "Available",
                    SerialNumber = "RT234trhnefs",
                    IsActive = true,
                }
            );
            context.equipments.Add(
                new Equipment
                {
                    EquipmentId = 1,
                    Name = "Equipment1",
                    Model = "Model1",
                    ImageURL = "url",
                    LabId = 1,
                }
            );
            context.labs.Add(
                new Lab
                {
                    LabId = 1,
                    LabName = "Lab1",
                    LabCode = "5674",
                }
            );
            context.SaveChanges();

            // Act
            var repository = new ItemRepository(context);
            var item = repository.GetItemDTOById(1);

            // Assert
            Assert.NotNull(item);
            Assert.Equal(1, item.itemId);
            Assert.Equal("Equipment1", item.itemName);
            Assert.Equal("Model1", item.itemModel);
            Assert.Equal("url", item.imageUrl);
            Assert.Equal(1, item.equipmentId);
            Assert.Equal(1, item.labId);
            Assert.Equal("Lab1", item.labName);
            Assert.Equal("RT234trhnefs", item.serialNumber);
            Assert.Equal("Available", item.status);
        }
    }

    // Test for GetAllItemDTOs
    [Fact]
    public void GetAllItemDTOs_ReturnsListOfItemDTOs()
    {
        // Arrange
        var options = CreateNewContextOptions();
        using (var context = new DataBaseContext(options))
        {
            context.items.Add(
                new Item
                {
                    ItemId = 1,
                    EquipmentId = 1,
                    Status = "Available",
                    SerialNumber = "RT234trhnefs",
                    IsActive = true,
                }
            );
            context.equipments.Add(
                new Equipment
                {
                    EquipmentId = 1,
                    Name = "Equipment1",
                    Model = "Model1",
                    ImageURL = "url",
                    LabId = 1,
                }
            );
            context.SaveChanges();

            // Act
            var repository = new ItemRepository(context);
            var items = repository.GetAllItemDTOs(1);

            // Assert
            Assert.NotNull(items);
            Assert.Single(items);
            Assert.Equal(1, items[0].itemId);
            Assert.Equal("url", items[0].imageUrl);
            Assert.Equal(1, items[0].equipmentId);
            Assert.Equal("RT234trhnefs", items[0].serialNumber);
            Assert.Equal("Available", items[0].status);
        }
    }

    // Test for CheckIfItemExists
    [Fact]
    public void CheckIfItemExists_ReturnsTrue()
    {
        // Arrange
        var options = CreateNewContextOptions();
        using (var context = new DataBaseContext(options))
        {
            context.items.Add(
                new Item
                {
                    ItemId = 1,
                    EquipmentId = 1,
                    Status = "Available",
                    SerialNumber = "RT234trhnefs",
                    IsActive = true,
                }
            );
            context.SaveChanges();

            // Act
            var repository = new ItemRepository(context);
            var itemExists = repository.CheckIfItemExists(1, "RT234trhnefs");

            // Assert
            Assert.True(itemExists);
        }
    }

    // Test for CreateNewItem
    [Fact]
    public void CreateNewItem_ReturnsItemDTO()
    {
        // Arrange
        var options = CreateNewContextOptions();
        using (var context = new DataBaseContext(options))
        {
            context.labs.Add(
                new Lab
                {
                    LabId = 1,
                    LabName = "Lab1",
                    LabCode = "5674",
                    IsActive = true,
                }
            );
            context.equipments.Add(
                new Equipment
                {
                    EquipmentId = 1,
                    Name = "Equipment1",
                    Model = "Model1",
                    ImageURL = "url",
                    LabId = 1,
                    IsActive = true,
                }
            );
            context.SaveChanges();

            // Act
            var repository = new ItemRepository(context);
            var item = repository.CreateNewItem(
                new CreateItemDTO { equipmentId = 1, serialNumber = "RT234trhnefs" },
                context.equipments.Find(1)
            );

            // Assert
            Assert.NotNull(item);
            Assert.Equal(1, item.itemId);
            Assert.Equal("Equipment1", item.itemName);
            Assert.Equal("Model1", item.itemModel);
            Assert.Equal("url", item.imageUrl);
            Assert.Equal(1, item.equipmentId);
            Assert.Equal(1, item.labId);
            Assert.Equal("RT234trhnefs", item.serialNumber);
            Assert.Equal("Available", item.status);
        }
    }

    // Test for CreateNewItem
    [Fact]
    public void CreateNewItem_ReturnsNull()
    {
        // Arrange
        var options = CreateNewContextOptions();
        using (var context = new DataBaseContext(options))
        {
            context.equipments.Add(
                new Equipment
                {
                    EquipmentId = 1,
                    Name = "Equipment1",
                    Model = "Model1",
                    ImageURL = "url",
                    LabId = 1,
                }
            );
            context.SaveChanges();

            // Act
            var repository = new ItemRepository(context);
            var item = repository.CreateNewItem(
                new CreateItemDTO { equipmentId = 1, serialNumber = "RT234trhnefs" },
                context.equipments.Find(2)
            );

            // Assert
            Assert.Null(item);
        }
    }
}
