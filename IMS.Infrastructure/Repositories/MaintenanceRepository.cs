using IMS.ApplicationCore.DTO;
using IMS.ApplicationCore.Interfaces;
using IMS.ApplicationCore.Model;
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
            return _dbContext.maintenances.Where(m => m.MaintenanceId == id && m.IsActive).FirstOrDefault();
        }

        public MaintenanceDetailedDTO? GetMaintenanceDTOById(int id)
        {
            return _dbContext.maintenances.Where(m => m.MaintenanceId == id && m.IsActive).Select(m => new MaintenanceDetailedDTO
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
                reviewedClerkName = m.ReviewedClerk != null ? m.ReviewedClerk.FirstName + " " + m.ReviewedClerk.LastName : null,
                reviewNote = m.ReviewNote,
                reviewedAt = m.ReviewedAt,
                cost = m.Cost,
                status = m.Status
            }).FirstOrDefault();
        }

        public List<MaintenanceDTO> GetAllMaintenanceDTOs(int itemId)
        {
            return _dbContext.maintenances.Where(m => m.IsActive && m.ItemId == itemId).Select(m => new MaintenanceDTO
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
                status = m.Status
            }).OrderByDescending(i => i.endDate).ToList();
        }

        public bool CheckTimeSlotAvailability(DateTime startDate, DateTime endDate)
        {
            return !_dbContext.maintenances.Any(m => m.EndDate >= startDate && m.StartDate <= endDate && m.IsActive && (m.Status != "Completed" && m.Status != "Canceled"));
        }

        public MaintenanceDetailedDTO? CreateNewMaintenance(CreateMaintenanceDTO createMaintenanceDTO, Item item, User clerk, User technician)
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
                IsActive = true
            };
            _dbContext.maintenances.Add(maintenance);
            _dbContext.SaveChanges();
            return GetMaintenanceDTOById(maintenance.MaintenanceId);
        }

        public MaintenanceDetailedDTO? ReviewMaintenance(Maintenance maintenance, ReviewMaintenanceDTO reviewMaintenanceDTO, User clerk, bool accepted)
        {
            if (maintenance == null) return null;
            maintenance.Status = accepted ? "Completed" : "Ongoing";
            maintenance.ReviewedClerkId = clerk.UserId;
            maintenance.ReviewedClerk = clerk;
            maintenance.ReviewNote = reviewMaintenanceDTO.reviewNote;
            maintenance.ReviewedAt = DateTime.Now;
            maintenance.Item.Status = accepted ? "Available" : "UnderRepair";
            _dbContext.Update(maintenance);
            _dbContext.Update(maintenance.Item);
            _dbContext.SaveChanges();
            return GetMaintenanceDTOById(maintenance.MaintenanceId);
        }

    }
}
