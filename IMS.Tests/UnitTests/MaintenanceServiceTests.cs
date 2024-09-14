using System.Collections.Generic;
using IMS.Application.DTO;
using IMS.Application.Interfaces;
using IMS.Application.Services;
using IMS.Core.Model;
using Moq;
using Xunit;

namespace IMS.Tests.UnitTests;

public class MaintenanceServiceTests
{
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<IItemRepository> _mockItemRepository;
    private readonly Mock<IMaintenanceRepository> _mockMaintenanceRepository;
    private readonly Mock<IReservationRepository> _mockReservationRepository;
    private readonly MaintenanceService _maintenanceService;

    public MaintenanceServiceTests()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _mockItemRepository = new Mock<IItemRepository>();
        _mockMaintenanceRepository = new Mock<IMaintenanceRepository>();
        _mockReservationRepository = new Mock<IReservationRepository>();
        _maintenanceService = new MaintenanceService(
            _mockUserRepository.Object,
            _mockItemRepository.Object,
            _mockMaintenanceRepository.Object,
            _mockReservationRepository.Object
        );
    }

    [Fact]
    public void GetMaintenanceById_ShouldReturnMaintenance_WhenFound()
    {
        // Arrange
        var maintenanceId = 1;
        var maintenanceDto = new MaintenanceDetailedDTO();
        _mockMaintenanceRepository
            .Setup(repo => repo.GetMaintenanceDTOById(maintenanceId))
            .Returns(maintenanceDto);

        // Act
        var result = _maintenanceService.GetMaintenanceById(maintenanceId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(maintenanceDto, result);
    }

    [Fact]
    public void GetMaintenanceById_ShouldReturnNull_WhenNotFound()
    {
        // Arrange
        var maintenanceId = 1;
        _mockMaintenanceRepository
            .Setup(repo => repo.GetMaintenanceDTOById(maintenanceId))
            .Returns((MaintenanceDetailedDTO?)null);

        // Act
        var result = _maintenanceService.GetMaintenanceById(maintenanceId);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void CreateNewMaintenance_ShouldReturnError_WhenClerkNotFound()
    {
        // Arrange
        int clerkId = 1;
        var createMaintenanceDTO = new CreateMaintenanceDTO();
        _mockUserRepository.Setup(repo => repo.GetUserEntityById(clerkId)).Returns((User?)null);

        // Act
        var result = _maintenanceService.CreateNewMaintenance(clerkId, createMaintenanceDTO);

        // Assert
        Assert.False(result.success);
        Assert.Equal("Clerk Not Found", result.message);
    }

    [Fact]
    public void CreateNewMaintenance_ShouldReturnError_WhenItemNotFound()
    {
        // Arrange
        int clerkId = 1;
        var clerk = new User { UserId = clerkId };
        var createMaintenanceDTO = new CreateMaintenanceDTO { itemId = 1 };
        _mockUserRepository.Setup(repo => repo.GetUserEntityById(clerkId)).Returns(clerk);
        _mockItemRepository
            .Setup(repo => repo.GetItemEntityById(createMaintenanceDTO.itemId))
            .Returns((Item?)null);

        // Act
        var result = _maintenanceService.CreateNewMaintenance(clerkId, createMaintenanceDTO);

        // Assert
        Assert.False(result.success);
        Assert.Equal("Item Not Found", result.message);
    }

    [Fact]
    public void CreateNewMaintenance_ShouldReturnSuccess_WhenValidData()
    {
        // Arrange
        int clerkId = 1;
        var clerk = new User { UserId = clerkId, Role = "Clerk" };
        var item = new Item { ItemId = 1, Status = "Available" };
        var technician = new User { UserId = 2, Role = "Technician" };
        var createMaintenanceDTO = new CreateMaintenanceDTO { itemId = 1, technicianId = 2 };
        var maintenanceDetailedDTO = new MaintenanceDetailedDTO();

        _mockUserRepository.Setup(repo => repo.GetUserEntityById(clerkId)).Returns(clerk);
        _mockItemRepository
            .Setup(repo => repo.GetItemEntityById(createMaintenanceDTO.itemId))
            .Returns(item);
        _mockUserRepository
            .Setup(repo => repo.GetUserEntityById(createMaintenanceDTO.technicianId))
            .Returns(technician);
        _mockMaintenanceRepository
            .Setup(repo =>
                repo.CheckTimeSlotAvailability(It.IsAny<DateTime>(), It.IsAny<DateTime>())
            )
            .Returns(true);
        _mockReservationRepository
            .Setup(repo =>
                repo.CheckTimeSlotAvailability(It.IsAny<DateTime>(), It.IsAny<DateTime>())
            )
            .Returns(true);
        _mockMaintenanceRepository
            .Setup(repo => repo.CreateNewMaintenance(item, clerk, technician, createMaintenanceDTO))
            .Returns(maintenanceDetailedDTO);

        // Act
        var result = _maintenanceService.CreateNewMaintenance(clerkId, createMaintenanceDTO);

        // Assert
        Assert.True(result.success);
        Assert.Equal(maintenanceDetailedDTO, result.result);
    }

    [Fact]
    public void BorrowItemForMaintenance_ShouldReturnError_WhenTechnicianNotFound()
    {
        // Arrange
        int maintenanceId = 1;
        int technicianId = 2;
        _mockUserRepository
            .Setup(repo => repo.GetUserEntityById(technicianId))
            .Returns((User?)null);

        // Act
        var result = _maintenanceService.BorrowItemForMaintenance(maintenanceId, technicianId);

        // Assert
        Assert.False(result.success);
        Assert.Equal("Technician Not Found", result.message);
    }

    [Fact]
    public void BorrowItemForMaintenance_ShouldReturnSuccess_WhenValidData()
    {
        // Arrange
        int maintenanceId = 1;
        int technicianId = 2;
        var technician = new User { UserId = technicianId, Role = "Technician" };
        var maintenance = new Maintenance
        {
            MaintenanceId = maintenanceId,
            Status = "Scheduled",
            TechnicianId = technicianId,
        };
        var maintenanceDetailedDTO = new MaintenanceDetailedDTO();

        _mockUserRepository.Setup(repo => repo.GetUserEntityById(technicianId)).Returns(technician);
        _mockMaintenanceRepository
            .Setup(repo => repo.GetMaintenanceEntityById(maintenanceId))
            .Returns(maintenance);
        _mockMaintenanceRepository
            .Setup(repo => repo.BorrowItemForMaintenance(maintenance))
            .Returns(maintenanceDetailedDTO);

        // Act
        var result = _maintenanceService.BorrowItemForMaintenance(maintenanceId, technicianId);

        // Assert
        Assert.True(result.success);
        Assert.Equal(maintenanceDetailedDTO, result.result);
    }

    [Fact]
    public void SubmitMaintenanceUpdate_ShouldReturnError_WhenMaintenanceNotFound()
    {
        // Arrange
        int maintenanceId = 1;
        int technicianId = 2;
        var technician = new User { UserId = technicianId, Role = "Technician" };
        var submitMaintenanceDTO = new SubmitMaintenanceDTO();
        _mockUserRepository.Setup(repo => repo.GetUserEntityById(technicianId)).Returns(technician);
        _mockMaintenanceRepository
            .Setup(repo => repo.GetMaintenanceEntityById(maintenanceId))
            .Returns((Maintenance?)null);

        // Act
        var result = _maintenanceService.SubmitMaintenanceUpdate(
            maintenanceId,
            technicianId,
            submitMaintenanceDTO
        );

        // Assert
        Assert.False(result.success);
        Assert.Equal("Maintenance Not Found", result.message);
    }

    [Fact]
    public void SubmitMaintenanceUpdate_ShouldReturnSuccess_WhenValidData()
    {
        // Arrange
        int maintenanceId = 1;
        int technicianId = 2;
        var technician = new User { UserId = technicianId, Role = "Technician" };
        var maintenance = new Maintenance
        {
            MaintenanceId = maintenanceId,
            Status = "Ongoing",
            TechnicianId = technicianId,
        };
        var submitMaintenanceDTO = new SubmitMaintenanceDTO();
        var updatedMaintenanceDTO = new MaintenanceDetailedDTO();

        _mockUserRepository.Setup(repo => repo.GetUserEntityById(technicianId)).Returns(technician);
        _mockMaintenanceRepository
            .Setup(repo => repo.GetMaintenanceEntityById(maintenanceId))
            .Returns(maintenance);
        _mockMaintenanceRepository
            .Setup(repo => repo.SubmitMaintenanceUpdate(maintenance, submitMaintenanceDTO))
            .Returns(updatedMaintenanceDTO);

        // Act
        var result = _maintenanceService.SubmitMaintenanceUpdate(
            maintenanceId,
            technicianId,
            submitMaintenanceDTO
        );

        // Assert
        Assert.True(result.success);
        Assert.Equal(updatedMaintenanceDTO, result.result);
    }

    [Fact]
    public void ReviewMaintenance_ShouldReturnError_WhenMaintenanceNotUnderReview()
    {
        // Arrange
        int maintenanceId = 1;
        int clerkId = 2;
        var clerk = new User { UserId = clerkId, Role = "Clerk" };
        var maintenance = new Maintenance { Status = "Ongoing" };
        var reviewMaintenanceDTO = new ReviewMaintenanceDTO();
        _mockUserRepository.Setup(repo => repo.GetUserEntityById(clerkId)).Returns(clerk);
        _mockMaintenanceRepository
            .Setup(repo => repo.GetMaintenanceEntityById(maintenanceId))
            .Returns(maintenance);

        // Act
        var result = _maintenanceService.ReviewMaintenance(
            maintenanceId,
            clerkId,
            reviewMaintenanceDTO
        );

        // Assert
        Assert.False(result.success);
        Assert.Equal("Maintenance is Not Under Review", result.message);
    }

    [Fact]
    public void ReviewMaintenance_ShouldReturnSuccess_WhenValidData()
    {
        // Arrange
        int maintenanceId = 1;
        int clerkId = 2;
        var clerk = new User { UserId = clerkId, Role = "Clerk" };
        var maintenance = new Maintenance { MaintenanceId = maintenanceId, Status = "UnderReview" };
        var reviewMaintenanceDTO = new ReviewMaintenanceDTO();
        var reviewedMaintenanceDTO = new MaintenanceDetailedDTO();

        _mockUserRepository.Setup(repo => repo.GetUserEntityById(clerkId)).Returns(clerk);
        _mockMaintenanceRepository
            .Setup(repo => repo.GetMaintenanceEntityById(maintenanceId))
            .Returns(maintenance);
        _mockMaintenanceRepository
            .Setup(repo => repo.ReviewMaintenance(maintenance, clerk, reviewMaintenanceDTO))
            .Returns(reviewedMaintenanceDTO);

        // Act
        var result = _maintenanceService.ReviewMaintenance(
            maintenanceId,
            clerkId,
            reviewMaintenanceDTO
        );

        // Assert
        Assert.True(result.success);
        Assert.Equal(reviewedMaintenanceDTO, result.result);
    }
}
