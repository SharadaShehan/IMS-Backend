using IMS.Application.DTO;
using IMS.Application.Interfaces;
using IMS.Core.Model;
using IMS.Infrastructure.Services;

namespace IMS.Infrastructure.Repositories
{
    public class MaintenanceRepository : IMaintenanceRepository
    {
        private readonly DataBaseContext _dbContext;

        public MaintenanceRepository(DataBaseContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Maintenance? GetMaintenanceEntityById(int id)
        {
            return _dbContext
                .maintenances.Where(m => m.MaintenanceId == id && m.IsActive)
                .FirstOrDefault();
        }

        public MaintenanceDetailedDTO? GetMaintenanceDTOById(int id)
        {
            return _dbContext
                .maintenances.Where(m => m.MaintenanceId == id && m.IsActive)
                .Select(m => new MaintenanceDetailedDTO
                {
                    maintenanceId = m.MaintenanceId,
                    itemId = m.ItemId,
                    itemName = m.Item.Equipment.Name,
                    itemModel = m.Item.Equipment.Model,
                    imageUrl = m.Item.Equipment.ImageURL,
                    itemSerialNumber = m.Item.SerialNumber,
                    labId = m.Item.Equipment.LabId,
                    labName = m.Item.Equipment.Lab.LabName,
                    startDate = m.StartDate,
                    endDate = m.EndDate,
                    createdClerkId = m.CreatedClerkId,
                    createdClerkName = m.CreatedClerk.FirstName + " " + m.CreatedClerk.LastName,
                    taskDescription = m.TaskDescription,
                    createdAt = m.CreatedAt,
                    technicianId = m.TechnicianId,
                    technicianName = m.Technician.FirstName + " " + m.Technician.LastName,
                    submitNote = m.SubmitNote,
                    submittedAt = m.SubmittedAt,
                    reviewedClerkId = m.ReviewedClerkId,
                    reviewedClerkName =
                        m.ReviewedClerk != null
                            ? m.ReviewedClerk.FirstName + " " + m.ReviewedClerk.LastName
                            : null,
                    reviewNote = m.ReviewNote,
                    reviewedAt = m.ReviewedAt,
                    cost = m.Cost,
                    status = m.Status,
                })
                .FirstOrDefault();
        }

        public List<MaintenanceDTO> GetAllMaintenanceDTOs(int itemId)
        {
            return _dbContext
                .maintenances.Where(m => m.IsActive && m.ItemId == itemId)
                .Select(m => new MaintenanceDTO
                {
                    maintenanceId = m.MaintenanceId,
                    itemId = m.ItemId,
                    itemName = m.Item.Equipment.Name,
                    itemModel = m.Item.Equipment.Model,
                    imageUrl = m.Item.Equipment.ImageURL,
                    itemSerialNumber = m.Item.SerialNumber,
                    labId = m.Item.Equipment.LabId,
                    labName = m.Item.Equipment.Lab.LabName,
                    startDate = m.StartDate,
                    endDate = m.EndDate,
                    createdAt = m.CreatedAt,
                    submittedAt = m.SubmittedAt,
                    reviewedAt = m.ReviewedAt,
                    status = m.Status,
                })
                .OrderByDescending(i => i.endDate)
                .ToList();
        }

        public List<MaintenanceDTO> GetAllCompletedMaintenanceDTOs()
        {
            return _dbContext
                .maintenances.Where(m => m.IsActive && m.Status == "Completed")
                .Select(m => new MaintenanceDTO
                {
                    maintenanceId = m.MaintenanceId,
                    itemId = m.ItemId,
                    itemName = m.Item.Equipment.Name,
                    itemModel = m.Item.Equipment.Model,
                    imageUrl = m.Item.Equipment.ImageURL,
                    itemSerialNumber = m.Item.SerialNumber,
                    labId = m.Item.Equipment.LabId,
                    labName = m.Item.Equipment.Lab.LabName,
                    startDate = m.StartDate,
                    endDate = m.EndDate,
                    createdAt = m.CreatedAt,
                    submittedAt = m.SubmittedAt,
                    reviewedAt = m.ReviewedAt,
                    status = m.Status,
                })
                .OrderByDescending(i => i.endDate)
                .ToList();
        }

        public List<MaintenanceDTO> GetAllNonCompletedMaintenanceDTOs()
        {
            return _dbContext
                .maintenances.Where(m =>
                    m.IsActive
                    && (
                        m.Status == "Ongoing"
                        || m.Status == "UnderReview"
                        || m.Status == "Scheduled"
                    )
                )
                .Select(m => new MaintenanceDTO
                {
                    maintenanceId = m.MaintenanceId,
                    itemId = m.ItemId,
                    itemName = m.Item.Equipment.Name,
                    itemModel = m.Item.Equipment.Model,
                    imageUrl = m.Item.Equipment.ImageURL,
                    itemSerialNumber = m.Item.SerialNumber,
                    labId = m.Item.Equipment.LabId,
                    labName = m.Item.Equipment.Lab.LabName,
                    startDate = m.StartDate,
                    endDate = m.EndDate,
                    createdAt = m.CreatedAt,
                    submittedAt = m.SubmittedAt,
                    reviewedAt = m.ReviewedAt,
                    status = m.Status,
                })
                .OrderByDescending(i => i.endDate)
                .ToList();
        }

        public List<MaintenanceDTO> GetAllCompletedMaintenanceDTOsByTechnicianId(int technicianId)
        {
            return _dbContext
                .maintenances.Where(m =>
                    m.IsActive && m.TechnicianId == technicianId && m.Status == "Completed"
                )
                .Select(m => new MaintenanceDTO
                {
                    maintenanceId = m.MaintenanceId,
                    itemId = m.ItemId,
                    itemName = m.Item.Equipment.Name,
                    itemModel = m.Item.Equipment.Model,
                    imageUrl = m.Item.Equipment.ImageURL,
                    itemSerialNumber = m.Item.SerialNumber,
                    labId = m.Item.Equipment.LabId,
                    labName = m.Item.Equipment.Lab.LabName,
                    startDate = m.StartDate,
                    endDate = m.EndDate,
                    createdAt = m.CreatedAt,
                    submittedAt = m.SubmittedAt,
                    reviewedAt = m.ReviewedAt,
                    status = m.Status,
                })
                .OrderByDescending(i => i.endDate)
                .ToList();
        }

        public List<MaintenanceDTO> GetAllNonCompletedMaintenanceDTOsByTechnicianId(
            int technicianId
        )
        {
            return _dbContext
                .maintenances.Where(m =>
                    m.IsActive
                    && m.TechnicianId == technicianId
                    && (
                        m.Status == "Ongoing"
                        || m.Status == "UnderReview"
                        || m.Status == "Scheduled"
                    )
                )
                .Select(m => new MaintenanceDTO
                {
                    maintenanceId = m.MaintenanceId,
                    itemId = m.ItemId,
                    itemName = m.Item.Equipment.Name,
                    itemModel = m.Item.Equipment.Model,
                    imageUrl = m.Item.Equipment.ImageURL,
                    itemSerialNumber = m.Item.SerialNumber,
                    labId = m.Item.Equipment.LabId,
                    labName = m.Item.Equipment.Lab.LabName,
                    startDate = m.StartDate,
                    endDate = m.EndDate,
                    createdAt = m.CreatedAt,
                    submittedAt = m.SubmittedAt,
                    reviewedAt = m.ReviewedAt,
                    status = m.Status,
                })
                .OrderByDescending(i => i.endDate)
                .ToList();
        }

        public List<PendingMaintenanceDTO> GetAllPendingMaintenanceDTOs()
        {
            // Get last maintenances and filter the pending ones
            return _dbContext
                .maintenances.GroupBy(mnt => mnt.ItemId)
                .Select(grp => grp.OrderByDescending(mnt => mnt.EndDate).FirstOrDefault())
                .Where(mnt =>
                    mnt != null
                    && mnt.IsActive
                    && (
                        mnt.EndDate.AddDays(mnt.Item.Equipment.MaintenanceIntervalDays ?? 10000)
                        < DateTime.Now
                    )
                )
                .Select(mnt => new PendingMaintenanceDTO
                {
                    itemId = mnt.ItemId,
                    itemName = mnt.Item.Equipment.Name,
                    itemModel = mnt.Item.Equipment.Model,
                    itemSerialNumber = mnt.Item.SerialNumber,
                    imageUrl = mnt.Item.Equipment.ImageURL,
                    LabId = mnt.Item.Equipment.LabId,
                    LabName = mnt.Item.Equipment.Lab.LabName,
                    lastMaintenanceId = mnt.MaintenanceId,
                    lastMaintenanceStartDate = mnt.StartDate,
                    lastMaintenanceEndDate = mnt.EndDate,
                })
                .ToList();
        }

        public bool CheckTimeSlotAvailability(DateTime startDate, DateTime endDate)
        {
            return !_dbContext.maintenances.Any(m =>
                m.EndDate >= startDate
                && m.StartDate <= endDate
                && m.IsActive
                && (m.Status != "Completed" && m.Status != "Canceled")
            );
        }

        public MaintenanceDetailedDTO? CreateNewMaintenance(
            Item item,
            User clerk,
            User technician,
            CreateMaintenanceDTO createMaintenanceDTO
        )
        {
            Maintenance maintenance = new Maintenance
            {
                ItemId = item.ItemId,
                Item = item,
                StartDate = createMaintenanceDTO.startDate,
                EndDate = createMaintenanceDTO.endDate,
                CreatedClerkId = clerk.UserId,
                CreatedClerk = clerk,
                TaskDescription = createMaintenanceDTO.taskDescription,
                CreatedAt = DateTime.Now,
                TechnicianId = technician.UserId,
                Technician = technician,
                Status = "Scheduled",
                IsActive = true,
            };
            _dbContext.maintenances.Add(maintenance);
            _dbContext.SaveChanges();
            return GetMaintenanceDTOById(maintenance.MaintenanceId);
        }

        public MaintenanceDetailedDTO? BorrowItemForMaintenance(Maintenance maintenance)
        {
            if (maintenance == null)
                return null;
            maintenance.Status = "Ongoing";
            maintenance.Item.Status = "UnderRepair";
            _dbContext.Update(maintenance);
            _dbContext.Update(maintenance.Item);
            _dbContext.SaveChanges();
            return GetMaintenanceDTOById(maintenance.MaintenanceId);
        }

        public MaintenanceDetailedDTO? SubmitMaintenanceUpdate(
            Maintenance maintenance,
            SubmitMaintenanceDTO submitMaintenanceDTO
        )
        {
            if (maintenance == null)
                return null;
            maintenance.SubmitNote = submitMaintenanceDTO.submitNote;
            maintenance.Cost = submitMaintenanceDTO.cost;
            maintenance.Status = "UnderReview";
            maintenance.SubmittedAt = DateTime.Now;
            _dbContext.Update(maintenance);
            _dbContext.SaveChanges();
            return GetMaintenanceDTOById(maintenance.MaintenanceId);
        }

        public MaintenanceDetailedDTO? ReviewMaintenance(
            Maintenance maintenance,
            User clerk,
            ReviewMaintenanceDTO reviewMaintenanceDTO
        )
        {
            if (maintenance == null)
                return null;
            maintenance.Status = reviewMaintenanceDTO.accepted ? "Completed" : "Ongoing";
            maintenance.ReviewedClerkId = clerk.UserId;
            maintenance.ReviewedClerk = clerk;
            maintenance.ReviewNote = reviewMaintenanceDTO.reviewNote;
            maintenance.ReviewedAt = DateTime.Now;
            maintenance.Item.Status = reviewMaintenanceDTO.accepted ? "Available" : "UnderRepair";
            _dbContext.Update(maintenance);
            _dbContext.Update(maintenance.Item);
            _dbContext.SaveChanges();
            return GetMaintenanceDTOById(maintenance.MaintenanceId);
        }
    }
}
