using System.Collections.Generic;
using IMS.Application.DTO;
using IMS.Application.Interfaces;
using IMS.Application.Services;
using IMS.Core.Model;
using Moq;
using Xunit;

namespace IMS.Tests.UnitTests;

public class EquipmentServiceTests
{
    private readonly Mock<IEquipmentRepository> _mockEquipmentRepository;
    private readonly Mock<ILabRepository> _mockLabRepository;
    private readonly EquipmentService _equipmentService;

    public EquipmentServiceTests()
    {
        _mockEquipmentRepository = new Mock<IEquipmentRepository>();
        _mockLabRepository = new Mock<ILabRepository>();
        _equipmentService = new EquipmentService(
            _mockEquipmentRepository.Object,
            _mockLabRepository.Object
        );
    }

    [Fact]
    public void GetEquipmentById_ExistingEquipment_ReturnsEquipment()
    {
        // Arrange
        var equipmentId = 1;
        var equipment = new EquipmentDetailedDTO
        {
            equipmentId = equipmentId,
            name = "Router",
            model = "Cisco",
            labId = 1,
            labName = "Network Lab",
            totalCount = 3,
            availableCount = 2,
            reservedCount = 0,
        };
        _mockEquipmentRepository
            .Setup(repo => repo.GetEquipmentDTOById(equipmentId))
            .Returns(equipment);

        // Act
        var result = _equipmentService.GetEquipmentById(equipmentId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(equipmentId, result.equipmentId);
    }

    [Fact]
    public void GetAllEquipments_ReturnsAllEquipments()
    {
        // Arrange
        var labId = 1;
        // Create a list of EquipmentDTOs
        var equipments = new List<EquipmentDTO>
        {
            new EquipmentDTO
            {
                equipmentId = 1,
                name = "Router",
                model = "Cisco",
                labId = 1,
                labName = "Network Lab",
            },
            new EquipmentDTO
            {
                equipmentId = 2,
                name = "Switch",
                model = "Cisco",
                labId = 1,
                labName = "Network Lab",
            },
        };
        _mockEquipmentRepository.Setup(repo => repo.GetAllEquipmentDTOs(labId)).Returns(equipments);

        // Act
        var result = _equipmentService.GetAllEquipments(labId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public void CreateNewEquipment_LabNotFound_ReturnsErrorResponse()
    {
        // Arrange
        var createEquipmentDTO = new CreateEquipmentDTO
        {
            labId = 1,
            name = "Test",
            model = "Model",
        };
        _mockLabRepository
            .Setup(repo => repo.GetLabEntityById(createEquipmentDTO.labId))
            .Returns((Lab)null);

        // Act
        var result = _equipmentService.CreateNewEquipment(createEquipmentDTO);

        // Assert
        Assert.NotNull(result);
        Assert.False(result.success);
        Assert.Equal("Lab Not Found", result.message);
    }

    [Fact]
    public void CreateNewEquipment_EquipmentAlreadyExists_ReturnsErrorResponse()
    {
        // Arrange
        var createEquipmentDTO = new CreateEquipmentDTO
        {
            labId = 1,
            name = "Test",
            model = "Model",
        };
        _mockLabRepository
            .Setup(repo => repo.GetLabEntityById(createEquipmentDTO.labId))
            .Returns(new Lab());
        _mockEquipmentRepository
            .Setup(repo =>
                repo.CheckIfEquipmentExists(
                    createEquipmentDTO.name,
                    createEquipmentDTO.model,
                    createEquipmentDTO.labId
                )
            )
            .Returns(true);

        // Act
        var result = _equipmentService.CreateNewEquipment(createEquipmentDTO);

        // Assert
        Assert.NotNull(result);
        Assert.False(result.success);
        Assert.Equal("Equipment Already Exists", result.message);
    }

    [Fact]
    public void CreateNewEquipment_Success_ReturnsCreatedEquipment()
    {
        // Arrange
        var createEquipmentDTO = new CreateEquipmentDTO
        {
            labId = 1,
            name = "Test",
            model = "Model",
        };
        var lab = new Lab();
        var equipmentDTO = new EquipmentDetailedDTO();
        _mockLabRepository
            .Setup(repo => repo.GetLabEntityById(createEquipmentDTO.labId))
            .Returns(lab);
        _mockEquipmentRepository
            .Setup(repo => repo.CreateNewEquipment(createEquipmentDTO, lab))
            .Returns(equipmentDTO);

        // Act
        var result = _equipmentService.CreateNewEquipment(createEquipmentDTO);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.success);
        Assert.Equal(equipmentDTO, result.result);
    }

    [Fact]
    public void UpdateEquipment_EquipmentNotFound_ReturnsErrorResponse()
    {
        // Arrange
        var equipmentId = 1;
        var updateEquipmentDTO = new UpdateEquipmentDTO();
        _mockEquipmentRepository
            .Setup(repo => repo.GetEquipmentEntityById(equipmentId))
            .Returns((Equipment)null);

        // Act
        var result = _equipmentService.UpdateEquipment(equipmentId, updateEquipmentDTO);

        // Assert
        Assert.NotNull(result);
        Assert.False(result.success);
        Assert.Equal("Equipment Not Found", result.message);
    }

    [Fact]
    public void UpdateEquipment_Success_ReturnsUpdatedEquipment()
    {
        // Arrange
        var equipmentId = 1;
        var lab = new Lab
        {
            LabId = 1,
            LabName = "Lab1",
            LabCode = "ICE356",
            ImageURL = "url",
            IsActive = true,
        };
        var equipment = new Equipment
        {
            EquipmentId = equipmentId,
            Name = "Router",
            Model = "Cisco",
            LabId = 1,
            Lab = lab,
            IsActive = true,
        };
        var updateEquipmentDTO = new UpdateEquipmentDTO { imageURL = "url" };
        var updatedEquipmentDTO = new EquipmentDetailedDTO
        {
            equipmentId = equipmentId,
            name = "Router",
            model = "Cisco",
            imageUrl = "url",
            labId = 1,
            labName = "Lab1",
        };
        _mockEquipmentRepository
            .Setup(repo => repo.GetEquipmentEntityById(equipmentId))
            .Returns(equipment);
        _mockEquipmentRepository
            .Setup(repo => repo.UpdateEquipment(equipment, updateEquipmentDTO))
            .Returns(updatedEquipmentDTO);

        // Act
        var result = _equipmentService.UpdateEquipment(equipmentId, updateEquipmentDTO);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.success);
        Assert.Equal(updatedEquipmentDTO, result.result);
    }

    [Fact]
    public void DeleteEquipment_EquipmentNotFound_ReturnsErrorResponse()
    {
        // Arrange
        var equipmentId = 1;
        _mockEquipmentRepository
            .Setup(repo => repo.GetEquipmentDTOById(equipmentId))
            .Returns((EquipmentDetailedDTO)null);

        // Act
        var result = _equipmentService.DeleteEquipment(equipmentId);

        // Assert
        Assert.NotNull(result);
        Assert.False(result.success);
        Assert.Equal("Equipment Not Found", result.message);
    }

    [Fact]
    public void DeleteEquipment_Success_ReturnsDeletedEquipment()
    {
        // Arrange
        var equipmentId = 1;
        var equipmentDTO = new EquipmentDetailedDTO();
        _mockEquipmentRepository
            .Setup(repo => repo.GetEquipmentDTOById(equipmentId))
            .Returns(equipmentDTO);
        _mockEquipmentRepository.Setup(repo => repo.DeleteEquipment(equipmentId)).Returns(true);

        // Act
        var result = _equipmentService.DeleteEquipment(equipmentId);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.success);
        Assert.Equal(equipmentDTO, result.result);
    }
}
