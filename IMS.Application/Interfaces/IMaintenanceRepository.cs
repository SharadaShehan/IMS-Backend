using IMS.Application.DTO;
using IMS.Core.Model;

namespace IMS.Application.Interfaces
{
    public interface IMaintenanceRepository
    {
        Maintenance? GetMaintenanceEntityById(int id);
        MaintenanceDetailedDTO? GetMaintenanceDTOById(int id);
        List<MaintenanceDTO> GetAllMaintenanceDTOs(int itemId);
        List<MaintenanceDTO> GetAllCompletedMaintenanceDTOs();
        List<MaintenanceDTO> GetAllNonCompletedMaintenanceDTOs();
        List<MaintenanceDTO> GetAllCompletedMaintenanceDTOsByTechnicianId(int technicianId);
        List<MaintenanceDTO> GetAllNonCompletedMaintenanceDTOsByTechnicianId(int technicianId);
        List<PendingMaintenanceDTO> GetAllPendingMaintenanceDTOs();
        bool CheckTimeSlotAvailability(DateTime startDate, DateTime endDate);
        MaintenanceDetailedDTO? CreateNewMaintenance(
            Item item,
            User clerk,
            User technician,
            CreateMaintenanceDTO createMaintenanceDTO
        );
        MaintenanceDetailedDTO? BorrowItemForMaintenance(Maintenance maintenance);
        MaintenanceDetailedDTO? SubmitMaintenanceUpdate(
            Maintenance maintenance,
            SubmitMaintenanceDTO submitMaintenanceDTO
        );
        MaintenanceDetailedDTO? ReviewMaintenance(
            Maintenance maintenance,
            User clerk,
            ReviewMaintenanceDTO reviewMaintenanceDTO
        );
    }
}
