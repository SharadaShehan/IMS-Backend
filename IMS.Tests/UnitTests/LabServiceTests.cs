using System.Collections.Generic;
using IMS.Application.DTO;
using IMS.Application.Interfaces;
using IMS.Application.Services;
using IMS.Core.Model;
using Moq;
using Xunit;

namespace IMS.Tests.UnitTests;

public class LabServiceTests
{
    private readonly Mock<ILabRepository> _mockLabRepository;
    private readonly LabService _labService;

    public LabServiceTests()
    {
        _mockLabRepository = new Mock<ILabRepository>();
        _labService = new LabService(_mockLabRepository.Object);
    }

    [Fact]
    public void GetLabById_ExistingLab_ReturnsLab()
    {
        // Arrange
        var labId = 1;
        var labDTO = new LabDTO
        {
            labId = 1,
            labName = "Lab1",
            labCode = "ICE356",
            imageUrl = "url",
        };
        _mockLabRepository.Setup(repo => repo.GetLabDTOById(labId)).Returns(labDTO);

        // Act
        var result = _labService.GetLabById(labId);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<LabDTO>(result);
        Assert.Equal(labId, result.labId);
        Assert.Equal("Lab1", labDTO.labName);
    }

    [Fact]
    public void GetAllLabs_ReturnsAllLabs()
    {
        // Arrange
        var labs = new List<LabDTO> { new LabDTO(), new LabDTO() };
        _mockLabRepository.Setup(repo => repo.GetAllLabDTOs()).Returns(labs);

        // Act
        var result = _labService.GetAllLabs();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public void CreateNewLab_LabAlreadyExists_ReturnsErrorResponse()
    {
        // Arrange
        var createLabDTO = new CreateLabDTO { labName = "TestLab", labCode = "TL01" };
        _mockLabRepository
            .Setup(repo => repo.CheckIfLabExists(createLabDTO.labName, createLabDTO.labCode))
            .Returns(true);

        // Act
        var result = _labService.CreateNewLab(createLabDTO);

        // Assert
        Assert.NotNull(result);
        Assert.False(result.success);
        Assert.Equal("Lab Already Exists", result.message);
    }

    [Fact]
    public void CreateNewLab_Success_ReturnsCreatedLab()
    {
        // Arrange
        var createLabDTO = new CreateLabDTO { labName = "TestLab", labCode = "TL01" };
        var labDTO = new LabDTO();
        _mockLabRepository
            .Setup(repo => repo.CheckIfLabExists(createLabDTO.labName, createLabDTO.labCode))
            .Returns(false);
        _mockLabRepository.Setup(repo => repo.CreateNewLab(createLabDTO)).Returns(labDTO);

        // Act
        var result = _labService.CreateNewLab(createLabDTO);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(labDTO, result.result);
    }

    [Fact]
    public void UpdateLab_LabNotFound_ReturnsErrorResponse()
    {
        // Arrange
        var labId = 1;
        var updateLabDTO = new UpdateLabDTO();
        _mockLabRepository.Setup(repo => repo.GetLabEntityById(labId)).Returns((Lab)null);

        // Act
        var result = _labService.UpdateLab(labId, updateLabDTO);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Lab Not Found", result.message);
    }

    [Fact]
    public void UpdateLab_Success_ReturnsUpdatedLab()
    {
        // Arrange
        var labId = 1;
        var lab = new Lab();
        var updateLabDTO = new UpdateLabDTO();
        var updatedLabDTO = new LabDTO();
        _mockLabRepository.Setup(repo => repo.GetLabEntityById(labId)).Returns(lab);
        _mockLabRepository.Setup(repo => repo.UpdateLab(lab, updateLabDTO)).Returns(updatedLabDTO);

        // Act
        var result = _labService.UpdateLab(labId, updateLabDTO);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(updatedLabDTO, result.result);
    }

    [Fact]
    public void DeleteLab_LabNotFound_ReturnsErrorResponse()
    {
        // Arrange
        var labId = 1;
        _mockLabRepository.Setup(repo => repo.GetLabDTOById(labId)).Returns((LabDTO)null);

        // Act
        var result = _labService.DeleteLab(labId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Lab Not Found", result.message);
    }

    [Fact]
    public void DeleteLab_Success_ReturnsDeletedLab()
    {
        // Arrange
        var labId = 1;
        var labDTO = new LabDTO();
        _mockLabRepository.Setup(repo => repo.GetLabDTOById(labId)).Returns(labDTO);
        _mockLabRepository.Setup(repo => repo.DeleteLab(labId)).Returns(true);

        // Act
        var result = _labService.DeleteLab(labId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(labDTO, result.result);
    }
}
