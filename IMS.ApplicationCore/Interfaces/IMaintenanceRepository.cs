using IMS.ApplicationCore.DTO;
using IMS.ApplicationCore.Model;

namespace IMS.ApplicationCore.Interfaces
{
    public interface IMaintenanceRepository
    {
        Maintenance? GetMaintenanceEntityById(int id);
        MaintenanceDetailedDTO? GetMaintenanceDTOById(int id);
        List<MaintenanceDTO> GetAllMaintenanceDTOs(int itemId);
        bool CheckTimeSlotAvailability(DateTime startDate, DateTime endDate);
        MaintenanceDetailedDTO? CreateNewMaintenance(CreateMaintenanceDTO createMaintenanceDTO, Item item, User clerk, User technician);
        MaintenanceDetailedDTO? ReviewMaintenance(Maintenance maintenance, ReviewMaintenanceDTO reviewMaintenanceDTO, User clerk, bool accepted);
    }
}
