using IMS.Application.DTO;
using IMS.Core.Model;
using IMS.Infrastructure.Repositories;
using IMS.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

namespace IMS.Tests.IntegrationTests;

public class MaintenanceRepositoryTests
{
    private DbContextOptions<DataBaseContext> CreateNewContextOptions()
    {
        return new DbContextOptionsBuilder<DataBaseContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Generates a unique database name
            .Options;
    }

    // Test for GetMaintenanceEntityById
    [Fact]
    public void GetMaintenanceEntityById_ReturnsMaintenanceEntity()
    {
        // Arrange
        var options = CreateNewContextOptions();
        using (var context = new DataBaseContext(options))
        {
            var repository = new MaintenanceRepository(context);
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
            var clerk = new User
            {
                UserId = 1,
                Email = "User@email.com",
                FirstName = "John",
                LastName = "Doe",
                ContactNumber = "123453647",
                Role = "Clerk",
                IsActive = true,
            };
            var technician = new User
            {
                UserId = 2,
                Email = "User2@email.com",
                FirstName = "Jane",
                LastName = "Doe",
                ContactNumber = "123903647",
                Role = "Technician",
                IsActive = true,
            };
            context.maintenances.Add(
                new Maintenance
                {
                    MaintenanceId = 1,
                    Item = item,
                    ItemId = item.ItemId,
                    StartDate = DateTime.Now.AddDays(3),
                    EndDate = DateTime.Now.AddDays(5),
                    CreatedClerkId = clerk.UserId,
                    CreatedClerk = clerk,
                    TaskDescription = "Test Repair",
                    CreatedAt = DateTime.Now.AddDays(-1),
                    TechnicianId = technician.UserId,
                    Technician = technician,
                    Status = "Scheduled",
                    IsActive = true,
                }
            );
            context.SaveChanges();

            // Act
            var maintenance = repository.GetMaintenanceEntityById(1);

            // Assert
            Assert.NotNull(maintenance);
            Assert.Equal(1, maintenance.MaintenanceId);
            Assert.Equal(1, maintenance.ItemId);
            Assert.Equal("Test Repair", maintenance.TaskDescription);
        }
    }

    // Test for GetMaintenanceDTOById
    [Fact]
    public void GetMaintenanceDTOById_ReturnsMaintenanceDetailedDTO()
    {
        // Arrange
        var options = CreateNewContextOptions();
        using (var context = new DataBaseContext(options))
        {
            var repository = new MaintenanceRepository(context);
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
            var clerk = new User
            {
                UserId = 1,
                Email = "User@email.com",
                FirstName = "John",
                LastName = "Doe",
                ContactNumber = "123453647",
                Role = "Clerk",
                IsActive = true,
            };
            var technician = new User
            {
                UserId = 2,
                Email = "User2@email.com",
                FirstName = "Jane",
                LastName = "Doe",
                ContactNumber = "123903647",
                Role = "Technician",
                IsActive = true,
            };
            context.maintenances.Add(
                new Maintenance
                {
                    MaintenanceId = 1,
                    Item = item,
                    ItemId = item.ItemId,
                    StartDate = DateTime.Now.AddDays(3),
                    EndDate = DateTime.Now.AddDays(5),
                    CreatedClerkId = clerk.UserId,
                    CreatedClerk = clerk,
                    TaskDescription = "Test Repair",
                    CreatedAt = DateTime.Now.AddDays(-1),
                    TechnicianId = technician.UserId,
                    Technician = technician,
                    Status = "Scheduled",
                    IsActive = true,
                }
            );
            context.SaveChanges();

            // Act
            var maintenanceDTO = repository.GetMaintenanceDTOById(1);

            // Assert
            Assert.NotNull(maintenanceDTO);
            Assert.Equal(1, maintenanceDTO.maintenanceId);
            Assert.Equal(1, maintenanceDTO.itemId);
            Assert.Equal("Test Repair", maintenanceDTO.taskDescription);
        }
    }

    // Test for GetAllMaintenanceDTOs
    [Fact]
    public void GetAllMaintenanceDTOs_ReturnsAllMaintenanceDTOs()
    {
        // Arrange
        var options = CreateNewContextOptions();
        using (var context = new DataBaseContext(options))
        {
            var repository = new MaintenanceRepository(context);
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
            var clerk = new User
            {
                UserId = 1,
                Email = "User@email.com",
                FirstName = "John",
                LastName = "Doe",
                ContactNumber = "123453647",
                Role = "Clerk",
                IsActive = true,
            };
            var technician = new User
            {
                UserId = 2,
                Email = "User2@email.com",
                FirstName = "Jane",
                LastName = "Doe",
                ContactNumber = "123903647",
                Role = "Technician",
                IsActive = true,
            };
            context.maintenances.Add(
                new Maintenance
                {
                    MaintenanceId = 1,
                    Item = item,
                    ItemId = item.ItemId,
                    StartDate = DateTime.Now.AddDays(3),
                    EndDate = DateTime.Now.AddDays(5),
                    CreatedClerkId = clerk.UserId,
                    CreatedClerk = clerk,
                    TaskDescription = "Test Repair",
                    CreatedAt = DateTime.Now.AddDays(-1),
                    TechnicianId = technician.UserId,
                    Technician = technician,
                    Status = "Scheduled",
                    IsActive = true,
                }
            );
            context.maintenances.Add(
                new Maintenance
                {
                    MaintenanceId = 2,
                    Item = item,
                    ItemId = item.ItemId,
                    StartDate = DateTime.Now.AddDays(-100),
                    EndDate = DateTime.Now.AddDays(-95),
                    CreatedClerkId = clerk.UserId,
                    CreatedClerk = clerk,
                    TaskDescription = "Test Repair Previous",
                    CreatedAt = DateTime.Now.AddDays(-120),
                    TechnicianId = technician.UserId,
                    Technician = technician,
                    Status = "Completed",
                    IsActive = true,
                }
            );
            context.SaveChanges();

            // Act
            var maintenances = repository.GetAllMaintenanceDTOs(1);

            // Assert
            Assert.Equal(2, maintenances.Count());
        }
    }

    // Test for GetAllCompletedMaintenanceDTOs
    [Fact]
    public void GetAllCompletedMaintenanceDTOs_ReturnsCompletedMaintenances()
    {
        // Arrange
        var options = CreateNewContextOptions();
        using (var context = new DataBaseContext(options))
        {
            var repository = new MaintenanceRepository(context);
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
            var clerk = new User
            {
                UserId = 1,
                Email = "User@email.com",
                FirstName = "John",
                LastName = "Doe",
                ContactNumber = "123453647",
                Role = "Clerk",
                IsActive = true,
            };
            var technician = new User
            {
                UserId = 2,
                Email = "User2@email.com",
                FirstName = "Jane",
                LastName = "Doe",
                ContactNumber = "123903647",
                Role = "Technician",
                IsActive = true,
            };
            context.maintenances.Add(
                new Maintenance
                {
                    MaintenanceId = 1,
                    Item = item,
                    ItemId = item.ItemId,
                    StartDate = DateTime.Now.AddDays(3),
                    EndDate = DateTime.Now.AddDays(5),
                    CreatedClerkId = clerk.UserId,
                    CreatedClerk = clerk,
                    TaskDescription = "Test Repair",
                    CreatedAt = DateTime.Now.AddDays(-1),
                    TechnicianId = technician.UserId,
                    Technician = technician,
                    Status = "Scheduled",
                    IsActive = true,
                }
            );
            context.maintenances.Add(
                new Maintenance
                {
                    MaintenanceId = 2,
                    Item = item,
                    ItemId = item.ItemId,
                    StartDate = DateTime.Now.AddDays(-100),
                    EndDate = DateTime.Now.AddDays(-95),
                    CreatedClerkId = clerk.UserId,
                    CreatedClerk = clerk,
                    TaskDescription = "Test Repair Previous",
                    CreatedAt = DateTime.Now.AddDays(-120),
                    TechnicianId = technician.UserId,
                    Technician = technician,
                    Status = "Completed",
                    IsActive = true,
                }
            );
            context.SaveChanges();

            // Act
            var completedMaintenances = repository.GetAllCompletedMaintenanceDTOs();

            // Assert
            Assert.Single(completedMaintenances);
            Assert.Equal("Completed", completedMaintenances.First().status);
        }
    }

    // Test for GetAllNonCompletedMaintenanceDTOs
    [Fact]
    public void GetAllNonCompletedMaintenanceDTOs_ReturnsNonCompletedMaintenances()
    {
        // Arrange
        var options = CreateNewContextOptions();
        using (var context = new DataBaseContext(options))
        {
            var repository = new MaintenanceRepository(context);
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
            var clerk = new User
            {
                UserId = 1,
                Email = "User@email.com",
                FirstName = "John",
                LastName = "Doe",
                ContactNumber = "123453647",
                Role = "Clerk",
                IsActive = true,
            };
            var technician = new User
            {
                UserId = 2,
                Email = "User2@email.com",
                FirstName = "Jane",
                LastName = "Doe",
                ContactNumber = "123903647",
                Role = "Technician",
                IsActive = true,
            };
            context.maintenances.Add(
                new Maintenance
                {
                    MaintenanceId = 1,
                    Item = item,
                    ItemId = item.ItemId,
                    StartDate = DateTime.Now.AddDays(3),
                    EndDate = DateTime.Now.AddDays(5),
                    CreatedClerkId = clerk.UserId,
                    CreatedClerk = clerk,
                    TaskDescription = "Test Repair",
                    CreatedAt = DateTime.Now.AddDays(-1),
                    TechnicianId = technician.UserId,
                    Technician = technician,
                    Status = "Ongoing",
                    IsActive = true,
                }
            );
            context.maintenances.Add(
                new Maintenance
                {
                    MaintenanceId = 2,
                    Item = item,
                    ItemId = item.ItemId,
                    StartDate = DateTime.Now.AddDays(-100),
                    EndDate = DateTime.Now.AddDays(-95),
                    CreatedClerkId = clerk.UserId,
                    CreatedClerk = clerk,
                    TaskDescription = "Test Repair Previous",
                    CreatedAt = DateTime.Now.AddDays(-120),
                    TechnicianId = technician.UserId,
                    Technician = technician,
                    Status = "Completed",
                    IsActive = true,
                }
            );
            context.SaveChanges();

            // Act
            var nonCompletedMaintenances = repository.GetAllNonCompletedMaintenanceDTOs();

            // Assert
            Assert.Single(nonCompletedMaintenances);
            Assert.Equal("Ongoing", nonCompletedMaintenances.First().status);
        }
    }

    // Test for GetAllMaintenanceDTOsByTechnicianId
    [Fact]
    public void GetAllCompletedMaintenanceDTOsByTechnicianId_ReturnsCompletedMaintenanceByTechnician()
    {
        // Arrange
        var options = CreateNewContextOptions();
        using (var context = new DataBaseContext(options))
        {
            var repository = new MaintenanceRepository(context);
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
            var clerk = new User
            {
                UserId = 1,
                Email = "User@email.com",
                FirstName = "John",
                LastName = "Doe",
                ContactNumber = "123453647",
                Role = "Clerk",
                IsActive = true,
            };
            var technician = new User
            {
                UserId = 2,
                Email = "User2@email.com",
                FirstName = "Jane",
                LastName = "Doe",
                ContactNumber = "123903647",
                Role = "Technician",
                IsActive = true,
            };
            var technician2 = new User
            {
                UserId = 3,
                Email = "User3@email.com",
                FirstName = "Jano",
                LastName = "Doe",
                ContactNumber = "123902347",
                Role = "Technician",
                IsActive = true,
            };
            context.maintenances.Add(
                new Maintenance
                {
                    MaintenanceId = 1,
                    Item = item,
                    ItemId = item.ItemId,
                    StartDate = DateTime.Now.AddDays(3),
                    EndDate = DateTime.Now.AddDays(5),
                    CreatedClerkId = clerk.UserId,
                    CreatedClerk = clerk,
                    TaskDescription = "Test Repair",
                    CreatedAt = DateTime.Now.AddDays(-1),
                    TechnicianId = technician.UserId,
                    Technician = technician,
                    Status = "Ongoing",
                    IsActive = true,
                }
            );
            context.maintenances.Add(
                new Maintenance
                {
                    MaintenanceId = 2,
                    Item = item,
                    ItemId = item.ItemId,
                    StartDate = DateTime.Now.AddDays(-100),
                    EndDate = DateTime.Now.AddDays(-95),
                    CreatedClerkId = clerk.UserId,
                    CreatedClerk = clerk,
                    TaskDescription = "Completed Repair",
                    CreatedAt = DateTime.Now.AddDays(-120),
                    TechnicianId = technician.UserId,
                    Technician = technician,
                    Status = "Completed",
                    IsActive = true,
                }
            );
            context.maintenances.Add(
                new Maintenance
                {
                    MaintenanceId = 3,
                    Item = item,
                    ItemId = item.ItemId,
                    StartDate = DateTime.Now.AddDays(-80),
                    EndDate = DateTime.Now.AddDays(-75),
                    CreatedClerkId = clerk.UserId,
                    CreatedClerk = clerk,
                    TaskDescription = "Completed Repair",
                    CreatedAt = DateTime.Now.AddDays(-100),
                    TechnicianId = technician2.UserId,
                    Technician = technician2,
                    Status = "Completed",
                    IsActive = true,
                }
            );
            context.SaveChanges();

            // Act
            var completedMaintenances = repository.GetAllCompletedMaintenanceDTOsByTechnicianId(2);

            // Assert
            Assert.Single(completedMaintenances);
            Assert.Equal("Completed", completedMaintenances.First().status);
        }
    }

    // Test for GetAllNonCompletedMaintenanceDTOsByTechnicianId
    [Fact]
    public void GetAllNonCompletedMaintenanceDTOsByTechnicianId_ReturnsNonCompletedMaintenanceByTechnician()
    {
        // Arrange
        var options = CreateNewContextOptions();
        using (var context = new DataBaseContext(options))
        {
            var repository = new MaintenanceRepository(context);
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
            var clerk = new User
            {
                UserId = 1,
                Email = "User@email.com",
                FirstName = "John",
                LastName = "Doe",
                ContactNumber = "123453647",
                Role = "Clerk",
                IsActive = true,
            };
            var technician = new User
            {
                UserId = 2,
                Email = "User2@email.com",
                FirstName = "Jane",
                LastName = "Doe",
                ContactNumber = "123903647",
                Role = "Technician",
                IsActive = true,
            };
            var technician2 = new User
            {
                UserId = 3,
                Email = "User3@email.com",
                FirstName = "Jano",
                LastName = "Doe",
                ContactNumber = "123902347",
                Role = "Technician",
                IsActive = true,
            };
            context.maintenances.Add(
                new Maintenance
                {
                    MaintenanceId = 1,
                    Item = item,
                    ItemId = item.ItemId,
                    StartDate = DateTime.Now.AddDays(3),
                    EndDate = DateTime.Now.AddDays(5),
                    CreatedClerkId = clerk.UserId,
                    CreatedClerk = clerk,
                    TaskDescription = "Test Repair",
                    CreatedAt = DateTime.Now.AddDays(-1),
                    TechnicianId = technician.UserId,
                    Technician = technician,
                    Status = "Ongoing",
                    IsActive = true,
                }
            );
            context.maintenances.Add(
                new Maintenance
                {
                    MaintenanceId = 2,
                    Item = item,
                    ItemId = item.ItemId,
                    StartDate = DateTime.Now.AddDays(-100),
                    EndDate = DateTime.Now.AddDays(-95),
                    CreatedClerkId = clerk.UserId,
                    CreatedClerk = clerk,
                    TaskDescription = "Completed Repair",
                    CreatedAt = DateTime.Now.AddDays(-120),
                    TechnicianId = technician.UserId,
                    Technician = technician,
                    Status = "Completed",
                    IsActive = true,
                }
            );
            context.maintenances.Add(
                new Maintenance
                {
                    MaintenanceId = 3,
                    Item = item,
                    ItemId = item.ItemId,
                    StartDate = DateTime.Now.AddDays(-80),
                    EndDate = DateTime.Now.AddDays(-75),
                    CreatedClerkId = clerk.UserId,
                    CreatedClerk = clerk,
                    TaskDescription = "Completed Repair",
                    CreatedAt = DateTime.Now.AddDays(-100),
                    TechnicianId = technician2.UserId,
                    Technician = technician2,
                    Status = "Completed",
                    IsActive = true,
                }
            );
            context.SaveChanges();

            // Act
            var nonCompletedMaintenances =
                repository.GetAllNonCompletedMaintenanceDTOsByTechnicianId(2);

            // Assert
            Assert.Single(nonCompletedMaintenances);
            Assert.Equal("Ongoing", nonCompletedMaintenances.First().status);
        }
    }

    // Test for CheckTimeSlotAvailability
    [Fact]
    public void CheckTimeSlotAvailability_ReturnsTrueIfTimeSlotIsAvailable()
    {
        // Arrange
        var options = CreateNewContextOptions();
        using (var context = new DataBaseContext(options))
        {
            var repository = new MaintenanceRepository(context);
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
            var clerk = new User
            {
                UserId = 1,
                Email = "User@email.com",
                FirstName = "John",
                LastName = "Doe",
                ContactNumber = "123453647",
                Role = "Clerk",
                IsActive = true,
            };
            var technician = new User
            {
                UserId = 2,
                Email = "User2@email.com",
                FirstName = "Jane",
                LastName = "Doe",
                ContactNumber = "123903647",
                Role = "Technician",
                IsActive = true,
            };
            context.maintenances.Add(
                new Maintenance
                {
                    MaintenanceId = 1,
                    Item = item,
                    ItemId = item.ItemId,
                    StartDate = DateTime.Now.AddDays(3),
                    EndDate = DateTime.Now.AddDays(5),
                    CreatedClerkId = clerk.UserId,
                    CreatedClerk = clerk,
                    TaskDescription = "Test Repair",
                    CreatedAt = DateTime.Now.AddDays(-1),
                    TechnicianId = technician.UserId,
                    Technician = technician,
                    Status = "Scheduled",
                    IsActive = true,
                }
            );
            context.SaveChanges();

            // Act
            var isAvailable = repository.CheckTimeSlotAvailability(
                DateTime.Now.AddDays(6),
                DateTime.Now.AddDays(8)
            );

            // Assert
            Assert.True(isAvailable);
        }
    }

    // Test for CheckTimeSlotAvailability
    [Fact]
    public void CheckTimeSlotAvailability_ReturnsFalseIfTimeSlotIsNotAvailable()
    {
        // Arrange
        var options = CreateNewContextOptions();
        using (var context = new DataBaseContext(options))
        {
            var repository = new MaintenanceRepository(context);
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
            var clerk = new User
            {
                UserId = 1,
                Email = "User@email.com",
                FirstName = "John",
                LastName = "Doe",
                ContactNumber = "123453647",
                Role = "Clerk",
                IsActive = true,
            };
            var technician = new User
            {
                UserId = 2,
                Email = "User2@email.com",
                FirstName = "Jane",
                LastName = "Doe",
                ContactNumber = "123903647",
                Role = "Technician",
                IsActive = true,
            };
            context.maintenances.Add(
                new Maintenance
                {
                    MaintenanceId = 1,
                    Item = item,
                    ItemId = item.ItemId,
                    StartDate = DateTime.Now.AddDays(3),
                    EndDate = DateTime.Now.AddDays(5),
                    CreatedClerkId = clerk.UserId,
                    CreatedClerk = clerk,
                    TaskDescription = "Test Repair",
                    CreatedAt = DateTime.Now.AddDays(-1),
                    TechnicianId = technician.UserId,
                    Technician = technician,
                    Status = "Scheduled",
                    IsActive = true,
                }
            );
            context.SaveChanges();

            // Act
            var isAvailable = repository.CheckTimeSlotAvailability(
                DateTime.Now.AddDays(4),
                DateTime.Now.AddDays(6)
            );

            // Assert
            Assert.False(isAvailable);
        }
    }

    // Test for CreateMaintenance
    [Fact]
    public void CreateMaintenance_ReturnsMaintenanceEntity()
    {
        // Arrange
        var options = CreateNewContextOptions();
        using (var context = new DataBaseContext(options))
        {
            var repository = new MaintenanceRepository(context);
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
            var clerk = new User
            {
                UserId = 1,
                Email = "User@email.com",
                FirstName = "John",
                LastName = "Doe",
                ContactNumber = "123453647",
                Role = "Clerk",
                IsActive = true,
            };
            var technician = new User
            {
                UserId = 2,
                Email = "User2@email.com",
                FirstName = "Jane",
                LastName = "Doe",
                ContactNumber = "123903647",
                Role = "Technician",
                IsActive = true,
            };

            // Act
            var maintenance = repository.CreateNewMaintenance(
                item,
                clerk,
                technician,
                new CreateMaintenanceDTO
                {
                    itemId = 1,
                    taskDescription = "Test Repair",
                    startDate = DateTime.Now.AddDays(3),
                    endDate = DateTime.Now.AddDays(5),
                }
            );

            // Assert
            Assert.NotNull(maintenance);
            Assert.Equal(1, maintenance.itemId);
            Assert.Equal("Test Repair", maintenance.taskDescription);
        }
    }

    // Test for BorrowItemForMaintenance
    [Fact]
    public void BorrowItemForMaintenance_ReturnsTrueIfItemIsBorrowedForMaintenance()
    {
        // Arrange
        var options = CreateNewContextOptions();
        using (var context = new DataBaseContext(options))
        {
            var repository = new MaintenanceRepository(context);
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
            var clerk = new User
            {
                UserId = 1,
                Email = "User@email.com",
                FirstName = "John",
                LastName = "Doe",
                ContactNumber = "123453647",
                Role = "Clerk",
                IsActive = true,
            };
            var technician = new User
            {
                UserId = 2,
                Email = "User2@email.com",
                FirstName = "Jane",
                LastName = "Doe",
                ContactNumber = "123903647",
                Role = "Technician",
                IsActive = true,
            };
            var maintenance = new Maintenance
            {
                MaintenanceId = 1,
                Item = item,
                ItemId = item.ItemId,
                StartDate = DateTime.Now.AddDays(3),
                EndDate = DateTime.Now.AddDays(5),
                CreatedClerkId = clerk.UserId,
                CreatedClerk = clerk,
                TaskDescription = "Test Repair",
                CreatedAt = DateTime.Now.AddDays(-1),
                TechnicianId = technician.UserId,
                Technician = technician,
                Status = "Scheduled",
                IsActive = true,
            };
            context.maintenances.Add(maintenance);
            context.SaveChanges();

            // Act
            var maintenanceDto = repository.BorrowItemForMaintenance(maintenance);

            // Assert
            Assert.NotNull(maintenanceDto);
            Assert.Equal("Ongoing", maintenanceDto.status);
        }
    }

    // Test for SubmitMaintenanceUpdate
    [Fact]
    public void SubmitMaintenanceUpdate_ReturnsMaintenanceEntityWithUpdatedDetails()
    {
        // Arrange
        var options = CreateNewContextOptions();
        using (var context = new DataBaseContext(options))
        {
            var repository = new MaintenanceRepository(context);
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
            var clerk = new User
            {
                UserId = 1,
                Email = "User@email.com",
                FirstName = "John",
                LastName = "Doe",
                ContactNumber = "123453647",
                Role = "Clerk",
                IsActive = true,
            };
            var technician = new User
            {
                UserId = 2,
                Email = "User2@email.com",
                FirstName = "Jane",
                LastName = "Doe",
                ContactNumber = "123903647",
                Role = "Technician",
                IsActive = true,
            };
            var maintenance = new Maintenance
            {
                MaintenanceId = 1,
                Item = item,
                ItemId = item.ItemId,
                StartDate = DateTime.Now.AddDays(3),
                EndDate = DateTime.Now.AddDays(5),
                CreatedClerkId = clerk.UserId,
                CreatedClerk = clerk,
                TaskDescription = "Test Repair",
                CreatedAt = DateTime.Now.AddDays(-1),
                TechnicianId = technician.UserId,
                Technician = technician,
                Status = "Ongoing",
                IsActive = true,
            };
            context.maintenances.Add(maintenance);
            context.SaveChanges();

            var submitMaintenanceDTO = new SubmitMaintenanceDTO
            {
                submitNote = "Test Update",
                cost = 100,
            };

            // Act
            var maintenanceDto = repository.SubmitMaintenanceUpdate(
                maintenance,
                submitMaintenanceDTO
            );

            // Assert
            Assert.NotNull(maintenanceDto);
            Assert.Equal("UnderReview", maintenanceDto.status);
            Assert.Equal("Test Update", maintenanceDto.submitNote);
            Assert.Equal(100, maintenanceDto.cost);
        }
    }

    // Test for ReviewMaintenance - Accept
    [Fact]
    public void ReviewMaintenance_ReturnsMaintenanceEntityWithUpdatedDetails()
    {
        // Arrange
        var options = CreateNewContextOptions();
        using (var context = new DataBaseContext(options))
        {
            var repository = new MaintenanceRepository(context);
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
            var clerk = new User
            {
                UserId = 1,
                Email = "User@email.com",
                FirstName = "John",
                LastName = "Doe",
                ContactNumber = "123453647",
                Role = "Clerk",
                IsActive = true,
            };
            var technician = new User
            {
                UserId = 2,
                Email = "User2@email.com",
                FirstName = "Jane",
                LastName = "Doe",
                ContactNumber = "123903647",
                Role = "Technician",
                IsActive = true,
            };
            var maintenance = new Maintenance
            {
                MaintenanceId = 1,
                Item = item,
                ItemId = item.ItemId,
                StartDate = DateTime.Now.AddDays(3),
                EndDate = DateTime.Now.AddDays(5),
                CreatedClerkId = clerk.UserId,
                CreatedClerk = clerk,
                TaskDescription = "Test Repair",
                CreatedAt = DateTime.Now.AddDays(-1),
                TechnicianId = technician.UserId,
                Technician = technician,
                Status = "UnderReview",
                IsActive = true,
            };
            context.maintenances.Add(maintenance);
            context.SaveChanges();

            var reviewMaintenanceDTO = new ReviewMaintenanceDTO { accepted = true };

            // Act
            var maintenanceDto = repository.ReviewMaintenance(
                maintenance,
                clerk,
                reviewMaintenanceDTO
            );

            // Assert
            Assert.NotNull(maintenanceDto);
            Assert.Equal("Completed", maintenanceDto.status);
        }
    }

    // Test for ReviewMaintenance - Reject
    [Fact]
    public void ReviewMaintenance_ReturnsMaintenanceEntityWithUpdatedDetailsIfRejected()
    {
        // Arrange
        var options = CreateNewContextOptions();
        using (var context = new DataBaseContext(options))
        {
            var repository = new MaintenanceRepository(context);
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
            var clerk = new User
            {
                UserId = 1,
                Email = "User@email.com",
                FirstName = "John",
                LastName = "Doe",
                ContactNumber = "123453647",
                Role = "Clerk",
                IsActive = true,
            };
            var technician = new User
            {
                UserId = 2,
                Email = "User2@email.com",
                FirstName = "Jane",
                LastName = "Doe",
                ContactNumber = "123903647",
                Role = "Technician",
                IsActive = true,
            };
            var maintenance = new Maintenance
            {
                MaintenanceId = 1,
                Item = item,
                ItemId = item.ItemId,
                StartDate = DateTime.Now.AddDays(3),
                EndDate = DateTime.Now.AddDays(5),
                CreatedClerkId = clerk.UserId,
                CreatedClerk = clerk,
                TaskDescription = "Test Repair",
                CreatedAt = DateTime.Now.AddDays(-1),
                TechnicianId = technician.UserId,
                Technician = technician,
                Status = "UnderReview",
                IsActive = true,
            };
            context.maintenances.Add(maintenance);
            context.SaveChanges();

            var reviewMaintenanceDTO = new ReviewMaintenanceDTO
            {
                accepted = false,
                reviewNote = "Test Reject",
            };

            // Act
            var maintenanceDto = repository.ReviewMaintenance(
                maintenance,
                clerk,
                reviewMaintenanceDTO
            );

            // Assert
            Assert.NotNull(maintenanceDto);
            Assert.Equal("Ongoing", maintenanceDto.status);
            Assert.Equal("Test Reject", maintenanceDto.reviewNote);
        }
    }
}
