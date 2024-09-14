using System.Collections.Generic;
using IMS.Application.DTO;
using IMS.Application.Interfaces;
using IMS.Application.Services;
using IMS.Core.Model;
using Moq;
using Xunit;

namespace IMS.Tests.UnitTests;

public class ReservationServiceTests
{
    private readonly Mock<IReservationRepository> _mockReservationRepository;
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<IEquipmentRepository> _mockEquipmentRepository;
    private readonly Mock<IItemRepository> _mockItemRepository;
    private readonly Mock<IMaintenanceRepository> _mockMaintenanceRepository;
    private readonly ReservationService _reservationService;

    public ReservationServiceTests()
    {
        _mockReservationRepository = new Mock<IReservationRepository>();
        _mockUserRepository = new Mock<IUserRepository>();
        _mockEquipmentRepository = new Mock<IEquipmentRepository>();
        _mockItemRepository = new Mock<IItemRepository>();
        _mockMaintenanceRepository = new Mock<IMaintenanceRepository>();
        _reservationService = new ReservationService(
            _mockReservationRepository.Object,
            _mockUserRepository.Object,
            _mockEquipmentRepository.Object,
            _mockItemRepository.Object,
            _mockMaintenanceRepository.Object
        );
    }

    [Fact]
    public void GetReservationById_ExistingReservation_ReturnsReservation()
    {
        // Arrange
        var reservationId = 1;
        var reservationDTO = new ItemReservationDetailedDTO { reservationId = reservationId };
        _mockReservationRepository
            .Setup(repo => repo.GetReservationDTOById(reservationId))
            .Returns(reservationDTO);

        // Act
        var result = _reservationService.GetReservationById(reservationId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(reservationId, result.reservationId);
    }

    [Fact]
    public void GetReservationById_ReservationNotFound_ReturnsNull()
    {
        // Arrange
        var reservationId = 1;
        _mockReservationRepository
            .Setup(repo => repo.GetReservationDTOById(reservationId))
            .Returns((ItemReservationDetailedDTO)null);

        // Act
        var result = _reservationService.GetReservationById(reservationId);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void CreateNewReservation_StudentNotFound_ReturnsError()
    {
        // Arrange
        var studentId = 1;
        var requestDTO = new RequestEquipmentDTO();
        _mockUserRepository.Setup(repo => repo.GetUserEntityById(studentId)).Returns((User)null);

        // Act
        var result = _reservationService.CreateNewReservation(studentId, requestDTO);

        // Assert
        Assert.Equal("Student Not Found", result.message);
    }

    [Fact]
    public void CreateNewReservation_EquipmentNotFound_ReturnsError()
    {
        // Arrange
        var student = new User { UserId = 1, Role = "Student" };
        var requestDTO = new RequestEquipmentDTO { equipmentId = 1 };
        _mockUserRepository.Setup(repo => repo.GetUserEntityById(student.UserId)).Returns(student);
        _mockEquipmentRepository
            .Setup(repo => repo.GetEquipmentEntityById(requestDTO.equipmentId))
            .Returns((Equipment)null);

        // Act
        var result = _reservationService.CreateNewReservation(student.UserId, requestDTO);

        // Assert
        Assert.Equal("Equipment Not Found", result.message);
    }

    [Fact]
    public void CreateNewReservation_TimeSlotUnavailable_ReturnsError()
    {
        // Arrange
        var student = new User { UserId = 1, Role = "Student" };
        var equipment = new Equipment { EquipmentId = 1 };
        var requestDTO = new RequestEquipmentDTO
        {
            equipmentId = equipment.EquipmentId,
            startDate = System.DateTime.Now,
            endDate = System.DateTime.Now.AddHours(1),
        };

        _mockUserRepository.Setup(repo => repo.GetUserEntityById(student.UserId)).Returns(student);
        _mockEquipmentRepository
            .Setup(repo => repo.GetEquipmentEntityById(equipment.EquipmentId))
            .Returns(equipment);
        _mockReservationRepository
            .Setup(repo => repo.CheckTimeSlotAvailability(requestDTO.startDate, requestDTO.endDate))
            .Returns(false);

        // Act
        var result = _reservationService.CreateNewReservation(student.UserId, requestDTO);

        // Assert
        Assert.Equal("Time Slot is Unavailable", result.message);
    }

    [Fact]
    public void CreateNewReservation_Success_ReturnsReservation()
    {
        // Arrange
        var student = new User { UserId = 1, Role = "Student" };
        var equipment = new Equipment { EquipmentId = 1 };
        var requestDTO = new RequestEquipmentDTO
        {
            equipmentId = equipment.EquipmentId,
            startDate = System.DateTime.Now,
            endDate = System.DateTime.Now.AddHours(1),
        };
        var reservationDTO = new ItemReservationDetailedDTO { equipmentId = 1 };

        _mockUserRepository.Setup(repo => repo.GetUserEntityById(student.UserId)).Returns(student);
        _mockEquipmentRepository
            .Setup(repo => repo.GetEquipmentEntityById(equipment.EquipmentId))
            .Returns(equipment);
        _mockReservationRepository
            .Setup(repo => repo.CheckTimeSlotAvailability(requestDTO.startDate, requestDTO.endDate))
            .Returns(true);
        _mockMaintenanceRepository
            .Setup(repo => repo.CheckTimeSlotAvailability(requestDTO.startDate, requestDTO.endDate))
            .Returns(true);
        _mockReservationRepository
            .Setup(repo => repo.RequestEquipmentReservation(equipment, student, requestDTO))
            .Returns(reservationDTO);

        // Act
        var result = _reservationService.CreateNewReservation(student.UserId, requestDTO);

        // Assert
        Assert.NotNull(result.result);
        Assert.Equal(reservationDTO.reservationId, result.result.reservationId);
    }

    [Fact]
    public void RespondToReservationRequest_ClerkNotFound_ReturnsError()
    {
        // Arrange
        _mockUserRepository.Setup(x => x.GetUserEntityById(It.IsAny<int>())).Returns((User)null);

        // Act
        var result = _reservationService.RespondToReservationRequest(
            1,
            1,
            new RespondReservationDTO()
        );

        // Assert
        Assert.False(result.success);
        Assert.Equal("Clerk Not Found", result.message);
    }

    [Fact]
    public void RespondToReservationRequest_ReservationNotPending_ReturnsError()
    {
        // Arrange
        var clerk = new User { UserId = 1, Role = "Clerk" };
        var reservation = new ItemReservation { ItemReservationId = 1, Status = "Approved" };

        _mockUserRepository.Setup(x => x.GetUserEntityById(It.IsAny<int>())).Returns(clerk);
        _mockReservationRepository
            .Setup(x => x.GetReservationEntityById(It.IsAny<int>()))
            .Returns(reservation);

        // Act
        var result = _reservationService.RespondToReservationRequest(
            1,
            1,
            new RespondReservationDTO { accepted = true }
        );

        // Assert
        Assert.False(result.success);
        Assert.Equal("Reservation is Not Pending", result.message);
    }

    [Fact]
    public void BorrowReservedItem_ClerkNotFound_ReturnsError()
    {
        // Arrange
        _mockUserRepository.Setup(x => x.GetUserEntityById(It.IsAny<int>())).Returns((User)null);

        // Act
        var result = _reservationService.BorrowReservedItem(1, 1);

        // Assert
        Assert.False(result.success);
        Assert.Equal("Clerk Not Found", result.message);
    }

    [Fact]
    public void BorrowReservedItem_ItemNotReserved_ReturnsError()
    {
        // Arrange
        var clerk = new User { UserId = 1, Role = "Clerk" };
        var reservation = new ItemReservation { ItemReservationId = 1, Status = "Pending" };

        _mockUserRepository.Setup(x => x.GetUserEntityById(It.IsAny<int>())).Returns(clerk);
        _mockReservationRepository
            .Setup(x => x.GetReservationEntityById(It.IsAny<int>()))
            .Returns(reservation);

        // Act
        var result = _reservationService.BorrowReservedItem(1, 1);

        // Assert
        Assert.False(result.success);
        Assert.Equal("Item is Not Reserved", result.message);
    }

    [Fact]
    public void CancelReservation_ReservationNotFound_ReturnsError()
    {
        // Arrange
        _mockReservationRepository
            .Setup(x => x.GetReservationEntityById(It.IsAny<int>()))
            .Returns((ItemReservation)null);

        // Act
        var result = _reservationService.CancelReservation(1, 1);

        // Assert
        Assert.False(result.success);
        Assert.Equal("Reservation Not Found", result.message);
    }

    [Fact]
    public void CancelReservation_NotOwner_ReturnsError()
    {
        // Arrange
        var reservation = new ItemReservation { ItemReservationId = 1, ReservedUserId = 2 };
        _mockReservationRepository
            .Setup(x => x.GetReservationEntityById(It.IsAny<int>()))
            .Returns(reservation);

        // Act
        var result = _reservationService.CancelReservation(1, 1);

        // Assert
        Assert.False(result.success);
        Assert.Equal("Only Reservation Owner can Cancel Reservation", result.message);
    }

    [Fact]
    public void ReturnBorrowedItem_ReservationNotBorrowed_ReturnsError()
    {
        // Arrange
        var clerk = new User { UserId = 1, Role = "Clerk" };
        var reservation = new ItemReservation { ItemReservationId = 1, Status = "Reserved" };

        _mockUserRepository.Setup(x => x.GetUserEntityById(It.IsAny<int>())).Returns(clerk);
        _mockReservationRepository
            .Setup(x => x.GetReservationEntityById(It.IsAny<int>()))
            .Returns(reservation);

        // Act
        var result = _reservationService.ReturnBorrowedItem(1, 1);

        // Assert
        Assert.False(result.success);
        Assert.Equal("Item is Not Borrowed", result.message);
    }

    [Fact]
    public void ReturnBorrowedItem_Success_ReturnsUpdatedReservation()
    {
        // Arrange
        var clerk = new User { UserId = 1, Role = "Clerk" };
        var reservation = new ItemReservation { ItemReservationId = 1, Status = "Borrowed" };
        var returnedReservation = new ItemReservationDetailedDTO();

        _mockUserRepository.Setup(x => x.GetUserEntityById(It.IsAny<int>())).Returns(clerk);
        _mockReservationRepository
            .Setup(x => x.GetReservationEntityById(It.IsAny<int>()))
            .Returns(reservation);
        _mockReservationRepository
            .Setup(x => x.ReturnBorrowedItem(It.IsAny<ItemReservation>(), It.IsAny<User>()))
            .Returns(returnedReservation);

        // Act
        var result = _reservationService.ReturnBorrowedItem(1, 1);

        // Assert
        Assert.True(result.success);
        Assert.Equal(returnedReservation, result.result);
    }
}
