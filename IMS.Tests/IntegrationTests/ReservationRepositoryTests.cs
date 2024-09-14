using IMS.Application.DTO;
using IMS.Core.Model;
using IMS.Infrastructure.Repositories;
using IMS.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

namespace IMS.Tests.IntegrationTests;

public class ReservationRepositoryTests
{
    private DbContextOptions<DataBaseContext> CreateNewContextOptions()
    {
        return new DbContextOptionsBuilder<DataBaseContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Generates a unique database name
            .Options;
    }

    // Test for GetReservationEntityById
    [Fact]
    public void GetReservationEntityById_ReturnsReservationEntity()
    {
        // Arrange
        var options = CreateNewContextOptions();
        using (var context = new DataBaseContext(options))
        {
            var repository = new ReservationRepository(context);

            context.itemReservations.Add(
                new ItemReservation
                {
                    ItemReservationId = 1,
                    EquipmentId = 1,
                    ItemId = 1,
                    StartDate = DateTime.Parse("2024-10-01"),
                    EndDate = DateTime.Parse("2024-10-03"),
                    ReservedUserId = 1,
                    CreatedAt = DateTime.Now,
                    Status = "Available",
                    IsActive = true,
                }
            );
            context.SaveChanges();

            // Act
            var reservation = repository.GetReservationEntityById(1);

            // Assert
            Assert.NotNull(reservation);
            Assert.Equal(1, reservation.ItemReservationId);
            Assert.Equal(1, reservation.EquipmentId);
            Assert.Equal(1, reservation.ItemId);
            Assert.Equal(DateTime.Parse("2024-10-01"), reservation.StartDate);
        }
    }

    // Test for GetReservationDTOById
    [Fact]
    public void GetReservationDTOById_ReturnsCorrectDTO()
    {
        // Arrange
        var options = CreateNewContextOptions();
        using (var context = new DataBaseContext(options))
        {
            var repository = new ReservationRepository(context);

            var equipment = new Equipment
            {
                EquipmentId = 1,
                Name = "Equipment1",
                Model = "Model1",
                LabId = 1,
                Lab = new Lab
                {
                    LabId = 1,
                    LabName = "Lab1",
                    LabCode = "12322",
                },
                ImageURL = "image.jpg",
                IsActive = true,
            };
            var reservedUser = new User
            {
                UserId = 1,
                Email = "User@email.com",
                FirstName = "John",
                LastName = "Doe",
                ContactNumber = "123453647",
                Role = "Admin",
                IsActive = true,
            };

            context.itemReservations.Add(
                new ItemReservation
                {
                    ItemReservationId = 1,
                    Equipment = equipment,
                    EquipmentId = equipment.EquipmentId,
                    StartDate = DateTime.Parse("2024-10-01"),
                    EndDate = DateTime.Parse("2024-10-03"),
                    ReservedUserId = reservedUser.UserId,
                    ReservedUser = reservedUser,
                    CreatedAt = DateTime.Now,
                    Status = "Pending",
                    IsActive = true,
                }
            );

            context.SaveChanges();

            // Act
            var reservationDTO = repository.GetReservationDTOById(1);

            // Assert
            Assert.NotNull(reservationDTO);
            Assert.Equal(1, reservationDTO.reservationId);
            Assert.Equal("Equipment1", reservationDTO.itemName);
            Assert.Equal("Lab1", reservationDTO.labName);
            Assert.Equal("John Doe", reservationDTO.reservedUserName);
        }
    }

    // Test for GetAllReservationDTOs
    [Fact]
    public void GetAllReservationDTOs_ReturnsAllReservationsForItem()
    {
        // Arrange
        var options = CreateNewContextOptions();
        using (var context = new DataBaseContext(options))
        {
            var repository = new ReservationRepository(context);
            var equipment = new Equipment
            {
                EquipmentId = 1,
                Name = "Equipment1",
                Model = "Model1",
                LabId = 1,
                Lab = new Lab
                {
                    LabId = 1,
                    LabName = "Lab1",
                    LabCode = "12322",
                },
                ImageURL = "image.jpg",
                IsActive = true,
            };
            var reservedUser = new User
            {
                UserId = 1,
                Email = "User@email.com",
                FirstName = "John",
                LastName = "Doe",
                ContactNumber = "123453647",
                Role = "Student",
                IsActive = true,
            };
            context.itemReservations.Add(
                new ItemReservation
                {
                    ItemReservationId = 1,
                    ItemId = 1,
                    Equipment = equipment,
                    EquipmentId = equipment.EquipmentId,
                    StartDate = DateTime.Parse("2024-10-01"),
                    EndDate = DateTime.Parse("2024-10-03"),
                    ReservedUserId = reservedUser.UserId,
                    ReservedUser = reservedUser,
                    CreatedAt = DateTime.Now,
                    Status = "Available",
                    IsActive = true,
                }
            );
            context.itemReservations.Add(
                new ItemReservation
                {
                    ItemReservationId = 2,
                    ItemId = 1,
                    Equipment = equipment,
                    EquipmentId = equipment.EquipmentId,
                    StartDate = DateTime.Parse("2024-11-01"),
                    EndDate = DateTime.Parse("2024-11-03"),
                    ReservedUserId = reservedUser.UserId,
                    ReservedUser = reservedUser,
                    CreatedAt = DateTime.Now,
                    Status = "Pending",
                    IsActive = true,
                }
            );
            context.SaveChanges();

            // Act
            var reservations = repository.GetAllReservationDTOs(1);

            // Assert
            Assert.Equal(2, reservations.Count);
            Assert.Contains(reservations, rsv => rsv.reservationId == 1);
            Assert.Contains(reservations, rsv => rsv.reservationId == 2);
        }
    }

    // Test for GetAllNonCompletedReservationDTOs
    [Fact]
    public void GetAllNonCompletedReservationDTOs_ReturnsNonCompletedReservations()
    {
        // Arrange
        var options = CreateNewContextOptions();
        using (var context = new DataBaseContext(options))
        {
            var repository = new ReservationRepository(context);
            var equipment = new Equipment
            {
                EquipmentId = 1,
                Name = "Equipment1",
                Model = "Model1",
                LabId = 1,
                Lab = new Lab
                {
                    LabId = 1,
                    LabName = "Lab1",
                    LabCode = "12322",
                },
                ImageURL = "image.jpg",
                IsActive = true,
            };
            var reservedUser = new User
            {
                UserId = 1,
                Email = "User@email.com",
                FirstName = "John",
                LastName = "Doe",
                ContactNumber = "123453647",
                Role = "Student",
                IsActive = true,
            };
            context.itemReservations.Add(
                new ItemReservation
                {
                    ItemReservationId = 1,
                    ItemId = 1,
                    Equipment = equipment,
                    EquipmentId = equipment.EquipmentId,
                    StartDate = DateTime.Parse("2024-10-01"),
                    EndDate = DateTime.Parse("2024-10-03"),
                    ReservedUserId = reservedUser.UserId,
                    ReservedUser = reservedUser,
                    CreatedAt = DateTime.Now,
                    Status = "Pending",
                    IsActive = true,
                }
            );
            context.itemReservations.Add(
                new ItemReservation
                {
                    ItemReservationId = 2,
                    ItemId = 1,
                    Equipment = equipment,
                    EquipmentId = equipment.EquipmentId,
                    StartDate = DateTime.Parse("2024-11-01"),
                    EndDate = DateTime.Parse("2024-11-03"),
                    ReservedUserId = reservedUser.UserId,
                    ReservedUser = reservedUser,
                    CreatedAt = DateTime.Now,
                    Status = "Completed",
                    IsActive = true,
                }
            );
            context.SaveChanges();

            // Act
            var nonCompletedReservations = repository.GetAllNonCompletedReservationDTOs();

            // Assert
            Assert.Single(nonCompletedReservations);
            Assert.Equal("Pending", nonCompletedReservations[0].status);
        }
    }

    // Test for GetAllRequestedReservationDTOs
    [Fact]
    public void GetAllRequestedReservationDTOs_ReturnsPendingReservations()
    {
        // Arrange
        var options = CreateNewContextOptions();
        using (var context = new DataBaseContext(options))
        {
            var repository = new ReservationRepository(context);
            var equipment = new Equipment
            {
                EquipmentId = 1,
                Name = "Projector",
                Model = "BenQ",
                LabId = 1,
                Lab = new Lab
                {
                    LabId = 1,
                    LabName = "Lab1",
                    LabCode = "12322",
                },
                IsActive = true,
            };
            var item = new Item
            {
                ItemId = 1,
                EquipmentId = 1,
                Equipment = equipment,
                Status = "Available",
                SerialNumber = "RT234trhnefs",
                IsActive = true,
            };
            var student = new User
            {
                UserId = 1,
                Email = "User2@email.com",
                FirstName = "Jane",
                LastName = "Doe",
                ContactNumber = "123452647",
                Role = "Student",
                IsActive = true,
            };
            var clerk = new User
            {
                UserId = 2,
                Email = "User@email.com",
                FirstName = "John",
                LastName = "Doe",
                ContactNumber = "123453647",
                Role = "Clerk",
                IsActive = true,
            };

            var reservation1 = new ItemReservation
            {
                ItemReservationId = 1,
                EquipmentId = equipment.EquipmentId,
                Equipment = equipment,
                ReservedUserId = student.UserId,
                ReservedUser = student,
                Status = "Pending",
                IsActive = true,
                StartDate = DateTime.Now.AddDays(1),
                EndDate = DateTime.Now.AddDays(2),
                CreatedAt = DateTime.Now,
            };

            var reservation2 = new ItemReservation
            {
                ItemReservationId = 2,
                EquipmentId = equipment.EquipmentId,
                Equipment = equipment,
                ReservedUserId = student.UserId,
                ReservedUser = student,
                RespondedClerk = clerk,
                RespondedClerkId = clerk.UserId,
                Item = item,
                ItemId = item.ItemId,
                Status = "Reserved",
                IsActive = true,
                StartDate = DateTime.Now.AddDays(3),
                EndDate = DateTime.Now.AddDays(5),
                CreatedAt = DateTime.Now.AddDays(-2),
                RespondedAt = DateTime.Now.AddDays(-1),
            };

            context.equipments.Add(equipment);
            context.users.Add(student);
            context.users.Add(clerk);
            context.itemReservations.AddRange(reservation1, reservation2);
            context.SaveChanges();

            // Act
            var result = repository.GetAllRequestedReservationDTOs();

            // Assert
            Assert.NotNull(result);
            Assert.Single(result); // Only the pending reservation should be returned
            var pendingReservation = result.First();
            Assert.Equal(reservation1.ItemReservationId, pendingReservation.reservationId);
            Assert.Equal(equipment.EquipmentId, pendingReservation.equipmentId);
            Assert.Equal(equipment.Name, pendingReservation.itemName);
            Assert.Equal(
                student.FirstName + " " + student.LastName,
                pendingReservation.reservedUserName
            );
            Assert.Equal("Pending", pendingReservation.status);
        }
    }

    // Test for GetAllReservedReservationDTOs
    [Fact]
    public void GetAllReservedReservationDTOs_ReturnsReservedReservations()
    {
        // Arrange
        var options = CreateNewContextOptions();
        using (var context = new DataBaseContext(options))
        {
            var repository = new ReservationRepository(context);
            var equipment = new Equipment
            {
                EquipmentId = 1,
                Name = "Projector",
                Model = "BenQ",
                LabId = 1,
                Lab = new Lab
                {
                    LabId = 1,
                    LabName = "Lab1",
                    LabCode = "12322",
                },
                IsActive = true,
            };
            var item = new Item
            {
                ItemId = 1,
                EquipmentId = 1,
                Equipment = equipment,
                Status = "Available",
                SerialNumber = "RT234trhnefs",
                IsActive = true,
            };
            var student = new User
            {
                UserId = 1,
                Email = "User2@email.com",
                FirstName = "Jane",
                LastName = "Doe",
                ContactNumber = "123452647",
                Role = "Student",
                IsActive = true,
            };
            var clerk = new User
            {
                UserId = 2,
                Email = "User@email.com",
                FirstName = "John",
                LastName = "Doe",
                ContactNumber = "123453647",
                Role = "Clerk",
                IsActive = true,
            };

            var reservedReservation = new ItemReservation
            {
                ItemReservationId = 1,
                EquipmentId = equipment.EquipmentId,
                Equipment = equipment,
                ReservedUserId = student.UserId,
                ReservedUser = student,
                RespondedClerk = clerk,
                RespondedClerkId = clerk.UserId,
                Item = item,
                ItemId = item.ItemId,
                Status = "Reserved",
                IsActive = true,
                StartDate = DateTime.Now.AddDays(3),
                EndDate = DateTime.Now.AddDays(5),
                CreatedAt = DateTime.Now.AddDays(-2),
                RespondedAt = DateTime.Now.AddDays(-1),
            };

            var nonReservedReservation = new ItemReservation
            {
                ItemReservationId = 2,
                EquipmentId = equipment.EquipmentId,
                Equipment = equipment,
                ReservedUserId = student.UserId,
                ReservedUser = student,
                Status = "Pending",
                IsActive = true,
                StartDate = DateTime.Now.AddDays(1),
                EndDate = DateTime.Now.AddDays(2),
                CreatedAt = DateTime.Now,
            };

            context.equipments.Add(equipment);
            context.users.Add(student);
            context.users.Add(clerk);
            context.itemReservations.AddRange(reservedReservation, nonReservedReservation);
            context.SaveChanges();

            // Act
            var result = repository.GetAllReservedReservationDTOs();

            // Assert
            Assert.NotNull(result);
            Assert.Single(result); // Only the reserved reservation should be returned
            var reservedResult = result.First();
            Assert.Equal(reservedReservation.ItemReservationId, reservedResult.reservationId);
            Assert.Equal(equipment.EquipmentId, reservedResult.equipmentId);
            Assert.Equal(equipment.Name, reservedResult.itemName);
            Assert.Equal(
                student.FirstName + " " + student.LastName,
                reservedResult.reservedUserName
            );
            Assert.Equal("Reserved", reservedResult.status);
            Assert.Equal(reservedReservation.RespondedAt, reservedResult.respondedAt);
        }
    }

    // Test for GetAllBorrowedReservationDTOs
    [Fact]
    public void GetAllBorrowedReservationDTOs_ReturnsBorrowedReservations()
    {
        // Arrange
        var options = CreateNewContextOptions();
        using (var context = new DataBaseContext(options))
        {
            var repository = new ReservationRepository(context);
            var equipment = new Equipment
            {
                EquipmentId = 1,
                Name = "Projector",
                Model = "BenQ",
                LabId = 1,
                Lab = new Lab
                {
                    LabId = 1,
                    LabName = "Lab1",
                    LabCode = "12322",
                },
                IsActive = true,
            };
            var item = new Item
            {
                ItemId = 1,
                EquipmentId = 1,
                Equipment = equipment,
                Status = "Available",
                SerialNumber = "RT234trhnefs",
                IsActive = true,
            };
            var student = new User
            {
                UserId = 1,
                Email = "User2@email.com",
                FirstName = "Jane",
                LastName = "Doe",
                ContactNumber = "123452647",
                Role = "Student",
                IsActive = true,
            };
            var clerk = new User
            {
                UserId = 2,
                Email = "User@email.com",
                FirstName = "John",
                LastName = "Doe",
                ContactNumber = "123453647",
                Role = "Clerk",
                IsActive = true,
            };

            var borrowedReservation = new ItemReservation
            {
                ItemReservationId = 1,
                EquipmentId = equipment.EquipmentId,
                Equipment = equipment,
                ReservedUserId = student.UserId,
                ReservedUser = student,
                RespondedClerk = clerk,
                RespondedClerkId = clerk.UserId,
                LentClerk = clerk,
                LentClerkId = clerk.UserId,
                Item = item,
                ItemId = item.ItemId,
                Status = "Borrowed",
                IsActive = true,
                StartDate = DateTime.Now.AddDays(3),
                EndDate = DateTime.Now.AddDays(5),
                CreatedAt = DateTime.Now.AddDays(-2),
                RespondedAt = DateTime.Now.AddDays(-1),
                BorrowedAt = DateTime.Now.AddDays(-1),
            };

            var nonBorrowedReservation = new ItemReservation
            {
                ItemReservationId = 2,
                EquipmentId = equipment.EquipmentId,
                Equipment = equipment,
                ReservedUserId = student.UserId,
                ReservedUser = student,
                Status = "Pending",
                IsActive = true,
                StartDate = DateTime.Now.AddDays(1),
                EndDate = DateTime.Now.AddDays(2),
                CreatedAt = DateTime.Now,
            };

            context.equipments.Add(equipment);
            context.users.Add(student);
            context.users.Add(clerk);
            context.itemReservations.AddRange(borrowedReservation, nonBorrowedReservation);
            context.SaveChanges();

            // Act
            var result = repository.GetAllBorrowedReservationDTOs();

            // Assert
            Assert.NotNull(result);
            Assert.Single(result); // Only the borrowed reservation should be returned
            var borrowedResult = result.First();
            Assert.Equal(borrowedReservation.ItemReservationId, borrowedResult.reservationId);
            Assert.Equal(equipment.EquipmentId, borrowedResult.equipmentId);
            Assert.Equal(equipment.Name, borrowedResult.itemName);
            Assert.Equal(
                student.FirstName + " " + student.LastName,
                borrowedResult.reservedUserName
            );
            Assert.Equal("Borrowed", borrowedResult.status);
            Assert.Equal(borrowedReservation.BorrowedAt, borrowedResult.borrowedAt);
        }
    }

    // Test for GetAllBorrowedReservationDTOsByStudent
    [Fact]
    public void GetAllBorrowedReservationDTOsByStudent_ReturnsBorrowedReservationsForStudent()
    {
        // Arrange
        var options = CreateNewContextOptions();
        using (var context = new DataBaseContext(options))
        {
            var repository = new ReservationRepository(context);
            var equipment = new Equipment
            {
                EquipmentId = 1,
                Name = "Equipment1",
                Model = "Model1",
                LabId = 1,
                Lab = new Lab
                {
                    LabId = 1,
                    LabName = "Lab1",
                    LabCode = "12322",
                },
                ImageURL = "image.jpg",
                IsActive = true,
            };
            var reservedUser1 = new User
            {
                UserId = 1,
                Email = "User@email.com",
                FirstName = "John",
                LastName = "Doe",
                ContactNumber = "123453647",
                Role = "Student",
                IsActive = true,
            };
            var reservedUser2 = new User
            {
                UserId = 2,
                Email = "User2@email.com",
                FirstName = "Jane",
                LastName = "Doe",
                ContactNumber = "123453647",
                Role = "Student",
                IsActive = true,
            };
            context.itemReservations.Add(
                new ItemReservation
                {
                    ItemReservationId = 1,
                    ItemId = 1,
                    Equipment = equipment,
                    EquipmentId = equipment.EquipmentId,
                    StartDate = DateTime.Parse("2024-10-01"),
                    EndDate = DateTime.Parse("2024-10-03"),
                    ReservedUserId = reservedUser1.UserId,
                    ReservedUser = reservedUser1,
                    CreatedAt = DateTime.Now,
                    Status = "Borrowed",
                    IsActive = true,
                }
            );
            context.itemReservations.Add(
                new ItemReservation
                {
                    ItemReservationId = 2,
                    ItemId = 1,
                    Equipment = equipment,
                    EquipmentId = equipment.EquipmentId,
                    StartDate = DateTime.Parse("2024-11-01"),
                    EndDate = DateTime.Parse("2024-11-03"),
                    ReservedUserId = reservedUser1.UserId,
                    ReservedUser = reservedUser1,
                    CreatedAt = DateTime.Now,
                    Status = "Pending",
                    IsActive = true,
                }
            );
            context.itemReservations.Add(
                new ItemReservation
                {
                    ItemReservationId = 3,
                    ItemId = 1,
                    Equipment = equipment,
                    EquipmentId = equipment.EquipmentId,
                    StartDate = DateTime.Parse("2024-11-05"),
                    EndDate = DateTime.Parse("2024-11-07"),
                    ReservedUserId = reservedUser2.UserId,
                    ReservedUser = reservedUser2,
                    CreatedAt = DateTime.Now,
                    Status = "Borrowed",
                    IsActive = true,
                }
            );
            context.SaveChanges();

            // Act
            var borrowedReservations = repository.GetAllBorrowedReservationDTOsByStudent(1);

            // Assert
            Assert.Single(borrowedReservations);
            Assert.Equal(1, borrowedReservations[0].reservationId);
            Assert.Equal("Borrowed", borrowedReservations[0].status);
        }
    }

    // Test for AcceptEquipmentReservation
    [Fact]
    public void AcceptEquipmentReservation_UpdatesStatusToReserved()
    {
        // Arrange
        var options = CreateNewContextOptions();
        using (var context = new DataBaseContext(options))
        {
            var repository = new ReservationRepository(context);
            var equipment = new Equipment
            {
                EquipmentId = 1,
                Name = "Projector",
                Model = "BenQ",
                LabId = 1,
                Lab = new Lab
                {
                    LabId = 1,
                    LabName = "Lab1",
                    LabCode = "12322",
                },
                IsActive = true,
            };
            var item = new Item
            {
                ItemId = 1,
                EquipmentId = 1,
                Equipment = equipment,
                Status = "Available",
                SerialNumber = "RT234trhnefs",
                IsActive = true,
            };
            var student = new User
            {
                UserId = 1,
                Email = "User2@email.com",
                FirstName = "Jane",
                LastName = "Doe",
                ContactNumber = "123452647",
                Role = "Student",
                IsActive = true,
            };
            var clerk = new User
            {
                UserId = 2,
                Email = "User@email.com",
                FirstName = "John",
                LastName = "Doe",
                ContactNumber = "123453647",
                Role = "Clerk",
                IsActive = true,
            };
            var reservation = new ItemReservation
            {
                ItemReservationId = 1,
                Equipment = equipment,
                EquipmentId = equipment.EquipmentId,
                StartDate = DateTime.Parse("2024-10-01"),
                EndDate = DateTime.Parse("2024-10-03"),
                ReservedUserId = student.UserId,
                ReservedUser = student,
                CreatedAt = DateTime.Now,
                Status = "Pending",
                IsActive = true,
            };

            var respondDTO = new RespondReservationDTO { };

            context.equipments.Add(equipment);
            context.items.Add(item);
            context.users.Add(student);
            context.users.Add(clerk);
            context.itemReservations.Add(reservation);
            context.SaveChanges();

            // Act
            var result = repository.AcceptEquipmentReservation(reservation, item, clerk);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Reserved", reservation.Status);
            Assert.Equal(clerk.UserId, reservation.RespondedClerkId);
            Assert.Equal("Reserved", result.status);
        }
    }

    // Test for RejectEquipmentReservation
    [Fact]
    public void RejectEquipmentReservation_UpdatesStatusToRejected()
    {
        // Arrange
        var options = CreateNewContextOptions();
        using (var context = new DataBaseContext(options))
        {
            var repository = new ReservationRepository(context);
            var equipment = new Equipment
            {
                EquipmentId = 1,
                Name = "Projector",
                Model = "BenQ",
                LabId = 1,
                Lab = new Lab
                {
                    LabId = 1,
                    LabName = "Lab1",
                    LabCode = "12322",
                },
                IsActive = true,
            };
            var student = new User
            {
                UserId = 1,
                Email = "User2@email.com",
                FirstName = "Jane",
                LastName = "Doe",
                ContactNumber = "123452647",
                Role = "Student",
                IsActive = true,
            };
            var clerk = new User
            {
                UserId = 2,
                Email = "User@email.com",
                FirstName = "John",
                LastName = "Doe",
                ContactNumber = "123453647",
                Role = "Clerk",
                IsActive = true,
            };
            var reservation = new ItemReservation
            {
                ItemReservationId = 1,
                Equipment = equipment,
                EquipmentId = equipment.EquipmentId,
                StartDate = DateTime.Parse("2024-10-01"),
                EndDate = DateTime.Parse("2024-10-03"),
                ReservedUserId = student.UserId,
                ReservedUser = student,
                CreatedAt = DateTime.Now,
                Status = "Pending",
                IsActive = true,
            };
            var respondDTO = new RespondReservationDTO { rejectNote = "Not available" };

            context.equipments.Add(equipment);
            context.users.Add(clerk);
            context.users.Add(student);
            context.itemReservations.Add(reservation);
            context.SaveChanges();

            // Act
            var result = repository.RejectEquipmentReservation(reservation, clerk, respondDTO);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Rejected", reservation.Status);
            Assert.Equal("Not available", reservation.ResponseNote);
            Assert.Equal(clerk.UserId, reservation.RespondedClerkId);
            Assert.Equal("Rejected", result.status);
        }
    }

    // Test for BorrowEquipment
    [Fact]
    public void BorrowReservedItem_UpdatesStatusToBorrowed()
    {
        // Arrange
        var options = CreateNewContextOptions();
        using (var context = new DataBaseContext(options))
        {
            var repository = new ReservationRepository(context);
            var equipment = new Equipment
            {
                EquipmentId = 1,
                Name = "Projector",
                Model = "BenQ",
                LabId = 1,
                Lab = new Lab
                {
                    LabId = 1,
                    LabName = "Lab1",
                    LabCode = "12322",
                },
                IsActive = true,
            };
            var item = new Item
            {
                ItemId = 1,
                EquipmentId = 1,
                Equipment = equipment,
                Status = "Available",
                SerialNumber = "RT234trhnefs",
                IsActive = true,
            };
            var student = new User
            {
                UserId = 1,
                Email = "User2@email.com",
                FirstName = "Jane",
                LastName = "Doe",
                ContactNumber = "123452647",
                Role = "Student",
                IsActive = true,
            };
            var clerk = new User
            {
                UserId = 2,
                Email = "User@email.com",
                FirstName = "John",
                LastName = "Doe",
                ContactNumber = "123453647",
                Role = "Clerk",
                IsActive = true,
            };
            var reservation = new ItemReservation
            {
                ItemReservationId = 1,
                Equipment = equipment,
                EquipmentId = equipment.EquipmentId,
                Item = item,
                ItemId = item.ItemId,
                StartDate = DateTime.Parse("2024-10-01"),
                EndDate = DateTime.Parse("2024-10-03"),
                ReservedUserId = student.UserId,
                ReservedUser = student,
                RespondedClerk = clerk,
                RespondedClerkId = clerk.UserId,
                RespondedAt = DateTime.Now,
                CreatedAt = DateTime.Now,
                Status = "Reserved",
                IsActive = true,
            };

            context.items.Add(item);
            context.users.Add(clerk);
            context.itemReservations.Add(reservation);
            context.SaveChanges();

            // Act
            var result = repository.BorrowReservedItem(reservation, clerk);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Borrowed", reservation.Status);
            Assert.Equal(clerk.UserId, reservation.LentClerkId);
            Assert.Equal("Borrowed", item.Status);
            Assert.Equal("Borrowed", result.status);
        }
    }

    // Test for CancelReservation
    [Fact]
    public void CancelReservation_UpdatesStatusToCanceled()
    {
        // Arrange
        var options = CreateNewContextOptions();
        using (var context = new DataBaseContext(options))
        {
            var repository = new ReservationRepository(context);
            var equipment = new Equipment
            {
                EquipmentId = 1,
                Name = "Projector",
                Model = "BenQ",
                LabId = 1,
                Lab = new Lab
                {
                    LabId = 1,
                    LabName = "Lab1",
                    LabCode = "12322",
                },
                IsActive = true,
            };
            var item = new Item
            {
                ItemId = 1,
                EquipmentId = 1,
                Equipment = equipment,
                Status = "Available",
                SerialNumber = "RT234trhnefs",
                IsActive = true,
            };
            var student = new User
            {
                UserId = 1,
                Email = "User2@email.com",
                FirstName = "Jane",
                LastName = "Doe",
                ContactNumber = "123452647",
                Role = "Student",
                IsActive = true,
            };
            var clerk = new User
            {
                UserId = 2,
                Email = "User@email.com",
                FirstName = "John",
                LastName = "Doe",
                ContactNumber = "123453647",
                Role = "Clerk",
                IsActive = true,
            };
            var reservation = new ItemReservation
            {
                ItemReservationId = 1,
                Equipment = equipment,
                EquipmentId = equipment.EquipmentId,
                Item = item,
                ItemId = item.ItemId,
                StartDate = DateTime.Parse("2024-10-01"),
                EndDate = DateTime.Parse("2024-10-03"),
                ReservedUserId = student.UserId,
                ReservedUser = student,
                RespondedClerk = clerk,
                RespondedClerkId = clerk.UserId,
                RespondedAt = DateTime.Now,
                CreatedAt = DateTime.Now,
                Status = "Reserved",
                IsActive = true,
            };

            context.items.Add(item);
            context.itemReservations.Add(reservation);
            context.SaveChanges();

            // Act
            var result = repository.CancelReservation(reservation);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Canceled", reservation.Status);
            Assert.False(reservation.IsActive);
            Assert.NotNull(reservation.CancelledAt);
            Assert.Equal("Canceled", result.status);
        }
    }

    // Test for ReturnBorrowedItem
    [Fact]
    public void ReturnBorrowedItem_UpdatesStatusToReturned()
    {
        // Arrange
        var options = CreateNewContextOptions();
        using (var context = new DataBaseContext(options))
        {
            var repository = new ReservationRepository(context);
            var equipment = new Equipment
            {
                EquipmentId = 1,
                Name = "Projector",
                Model = "BenQ",
                LabId = 1,
                Lab = new Lab
                {
                    LabId = 1,
                    LabName = "Lab1",
                    LabCode = "12322",
                },
                IsActive = true,
            };
            var item = new Item
            {
                ItemId = 1,
                EquipmentId = 1,
                Equipment = equipment,
                Status = "Borrowed",
                SerialNumber = "RT234trhnefs",
                IsActive = true,
            };
            var student = new User
            {
                UserId = 1,
                Email = "User2@email.com",
                FirstName = "Jane",
                LastName = "Doe",
                ContactNumber = "123452647",
                Role = "Student",
                IsActive = true,
            };
            var clerk = new User
            {
                UserId = 2,
                Email = "User@email.com",
                FirstName = "John",
                LastName = "Doe",
                ContactNumber = "123453647",
                Role = "Clerk",
                IsActive = true,
            };
            var reservation = new ItemReservation
            {
                ItemReservationId = 1,
                Equipment = equipment,
                EquipmentId = equipment.EquipmentId,
                Item = item,
                ItemId = item.ItemId,
                StartDate = DateTime.Parse("2024-10-01"),
                EndDate = DateTime.Parse("2024-10-03"),
                ReservedUserId = student.UserId,
                ReservedUser = student,
                RespondedClerk = clerk,
                RespondedClerkId = clerk.UserId,
                RespondedAt = DateTime.Now,
                LentClerk = clerk,
                LentClerkId = clerk.UserId,
                BorrowedAt = DateTime.Now,
                CreatedAt = DateTime.Now,
                Status = "Borrowed",
                IsActive = true,
            };

            context.items.Add(item);
            context.itemReservations.Add(reservation);
            context.users.Add(clerk);
            context.SaveChanges();

            // Act
            var result = repository.ReturnBorrowedItem(reservation, clerk);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Returned", reservation.Status);
            Assert.Equal(clerk.UserId, reservation.ReturnAcceptedClerkId);
            Assert.Equal("Available", item.Status);
            Assert.NotNull(reservation.ReturnedAt);
            Assert.Equal("Returned", result.status);
        }
    }

    // Test for CheckTimeSlotAvailability
    [Fact]
    public void CheckTimeSlotAvailability_ReturnsFalseWhenTimeSlotIsOccupied()
    {
        // Arrange
        var options = CreateNewContextOptions();
        using (var context = new DataBaseContext(options))
        {
            var repository = new ReservationRepository(context);
            var equipment = new Equipment
            {
                EquipmentId = 1,
                Name = "Equipment1",
                Model = "Model1",
                LabId = 1,
                Lab = new Lab
                {
                    LabId = 1,
                    LabName = "Lab1",
                    LabCode = "12322",
                },
                ImageURL = "image.jpg",
                IsActive = true,
            };
            var reservedUser = new User
            {
                UserId = 1,
                Email = "User@email.com",
                FirstName = "John",
                LastName = "Doe",
                ContactNumber = "123453647",
                Role = "Admin",
                IsActive = true,
            };
            context.itemReservations.Add(
                new ItemReservation
                {
                    ItemReservationId = 1,
                    ItemId = 1,
                    Equipment = equipment,
                    EquipmentId = equipment.EquipmentId,
                    StartDate = DateTime.Parse("2024-10-01"),
                    EndDate = DateTime.Parse("2024-10-03"),
                    ReservedUserId = reservedUser.UserId,
                    ReservedUser = reservedUser,
                    CreatedAt = DateTime.Now,
                    Status = "Reserved",
                    IsActive = true,
                }
            );
            context.SaveChanges();

            // Act
            var isAvailable = repository.CheckTimeSlotAvailability(
                DateTime.Parse("2024-10-02"),
                DateTime.Parse("2024-10-04")
            );

            // Assert
            Assert.False(isAvailable);
        }
    }
}
