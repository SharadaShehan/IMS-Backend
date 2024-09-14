using System.Collections.Generic;
using IMS.Application.DTO;
using IMS.Application.Interfaces;
using IMS.Application.Services;
using IMS.Core.Model;
using Moq;
using Xunit;

namespace IMS.Tests.UnitTests;

public class ItemServiceTests
{
    private readonly Mock<IItemRepository> _mockItemRepository;
    private readonly Mock<IEquipmentRepository> _mockEquipmentRepository;
    private readonly ItemService _itemService;

    public ItemServiceTests()
    {
        _mockItemRepository = new Mock<IItemRepository>();
        _mockEquipmentRepository = new Mock<IEquipmentRepository>();
        _itemService = new ItemService(_mockItemRepository.Object, _mockEquipmentRepository.Object);
    }

    [Fact]
    public void GetItemById_ExistingItem_ReturnsItem()
    {
        // Arrange
        var itemId = 1;
        var itemDTO = new ItemDetailedDTO { itemId = itemId };
        _mockItemRepository.Setup(repo => repo.GetItemDTOById(itemId)).Returns(itemDTO);

        // Act
        var result = _itemService.GetItemById(itemId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(itemId, result.itemId);
    }

    [Fact]
    public void GetAllItems_ReturnsAllItems()
    {
        // Arrange
        var equipmentId = 1;
        var items = new List<ItemDTO>
        {
            new ItemDTO { itemId = 1 },
            new ItemDTO { itemId = 2 },
        };
        _mockItemRepository.Setup(repo => repo.GetAllItemDTOs(equipmentId)).Returns(items);

        // Act
        var result = _itemService.GetAllItems(equipmentId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public void CreateNewItem_EquipmentNotFound_ReturnsErrorResponse()
    {
        // Arrange
        var createItemDTO = new CreateItemDTO { equipmentId = 1, serialNumber = "SN001" };
        _mockEquipmentRepository
            .Setup(repo => repo.GetEquipmentEntityById(createItemDTO.equipmentId))
            .Returns((Equipment)null);

        // Act
        var result = _itemService.CreateNewItem(createItemDTO);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Equipment Not Found", result.message);
    }

    [Fact]
    public void CreateNewItem_ItemAlreadyExists_ReturnsErrorResponse()
    {
        // Arrange
        var createItemDTO = new CreateItemDTO { equipmentId = 1, serialNumber = "SN001" };
        var equipment = new Equipment();
        _mockEquipmentRepository
            .Setup(repo => repo.GetEquipmentEntityById(createItemDTO.equipmentId))
            .Returns(equipment);
        _mockItemRepository
            .Setup(repo =>
                repo.CheckIfItemExists(createItemDTO.equipmentId, createItemDTO.serialNumber)
            )
            .Returns(true);

        // Act
        var result = _itemService.CreateNewItem(createItemDTO);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Item Already Exists", result.message);
    }

    [Fact]
    public void CreateNewItem_Success_ReturnsCreatedItem()
    {
        // Arrange
        var createItemDTO = new CreateItemDTO { equipmentId = 1, serialNumber = "SN001" };
        var equipment = new Equipment();
        var itemDTO = new ItemDetailedDTO();
        _mockEquipmentRepository
            .Setup(repo => repo.GetEquipmentEntityById(createItemDTO.equipmentId))
            .Returns(equipment);
        _mockItemRepository
            .Setup(repo =>
                repo.CheckIfItemExists(createItemDTO.equipmentId, createItemDTO.serialNumber)
            )
            .Returns(false);
        _mockItemRepository
            .Setup(repo => repo.CreateNewItem(createItemDTO, equipment))
            .Returns(itemDTO);

        // Act
        var result = _itemService.CreateNewItem(createItemDTO);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(itemDTO, result.result);
    }

    [Fact]
    public void DeleteItem_ItemNotFound_ReturnsErrorResponse()
    {
        // Arrange
        var itemId = 1;
        _mockItemRepository
            .Setup(repo => repo.GetItemDTOById(itemId))
            .Returns((ItemDetailedDTO)null);

        // Act
        var result = _itemService.DeleteItem(itemId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Item Not Found", result.message);
    }

    [Fact]
    public void DeleteItem_Success_ReturnsDeletedItem()
    {
        // Arrange
        var itemId = 1;
        var itemDTO = new ItemDetailedDTO();
        _mockItemRepository.Setup(repo => repo.GetItemDTOById(itemId)).Returns(itemDTO);
        _mockItemRepository.Setup(repo => repo.DeleteItem(itemId)).Returns(true);

        // Act
        var result = _itemService.DeleteItem(itemId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(itemDTO, result.result);
    }
}
