using IMS.Application.DTO;
using IMS.Application.Interfaces;
using IMS.Core.Model;

namespace IMS.Application.Services
{
    public class MaintenanceService
    {
        private readonly IUserRepository _userRepository;
        private readonly IItemRepository _itemRepository;
        private readonly IMaintenanceRepository _maintenanceRepository;
        private readonly IReservationRepository _reservationRepository;

        public MaintenanceService(
            IUserRepository userRepository,
            IItemRepository itemRepository,
            IMaintenanceRepository maintenanceRepository,
            IReservationRepository reservationRepository
        )
        {
            _userRepository = userRepository;
            _itemRepository = itemRepository;
            _maintenanceRepository = maintenanceRepository;
            _reservationRepository = reservationRepository;
        }

        public MaintenanceDetailedDTO? GetMaintenanceById(int id)
        {
            return _maintenanceRepository.GetMaintenanceDTOById(id);
        }

        public List<MaintenanceDTO> GetAllMaintenances(int itemId)
        {
            return _maintenanceRepository.GetAllMaintenanceDTOs(itemId);
        }

        public List<MaintenanceDTO> GetAllMaintenances(bool completed)
        {
            if (completed)
                return _maintenanceRepository.GetAllCompletedMaintenanceDTOs();
            else
                return _maintenanceRepository.GetAllNonCompletedMaintenanceDTOs();
        }

        public List<MaintenanceDTO> GetAllMaintenancesByTechnicianId(
            int technicianId,
            bool completed
        )
        {
            if (completed)
                return _maintenanceRepository.GetAllCompletedMaintenanceDTOsByTechnicianId(
                    technicianId
                );
            else
                return _maintenanceRepository.GetAllNonCompletedMaintenanceDTOsByTechnicianId(
                    technicianId
                );
        }

        public List<PendingMaintenanceDTO> GetAllPendingMaintenances()
        {
            return _maintenanceRepository.GetAllPendingMaintenanceDTOs();
        }

        public ResponseDTO<MaintenanceDetailedDTO> CreateNewMaintenance(
            int clerkId,
            CreateMaintenanceDTO createMaintenanceDTO
        )
        {
            // Check if Clerk Exists
            User? clerk = _userRepository.GetUserEntityById(clerkId);
            if (clerk == null)
                return new ResponseDTO<MaintenanceDetailedDTO>("Clerk Not Found");
            // Check if Item Exists
            Item? item = _itemRepository.GetItemEntityById(createMaintenanceDTO.itemId);
            if (item == null)
                return new ResponseDTO<MaintenanceDetailedDTO>("Item Not Found");
            if (item.Status == "Unavailable")
                return new ResponseDTO<MaintenanceDetailedDTO>("Item is Unavailable");
            // Check if Technician Exists
            User? technician = _userRepository.GetUserEntityById(createMaintenanceDTO.technicianId);
            if (technician == null)
                return new ResponseDTO<MaintenanceDetailedDTO>("Technician Not Found");
            // Check if Time Slot is Available
            if (
                !_maintenanceRepository.CheckTimeSlotAvailability(
                    createMaintenanceDTO.startDate,
                    createMaintenanceDTO.endDate
                )
            )
                return new ResponseDTO<MaintenanceDetailedDTO>("Time Slot Not Available");
            if (
                !_reservationRepository.CheckTimeSlotAvailability(
                    createMaintenanceDTO.startDate,
                    createMaintenanceDTO.endDate
                )
            )
                return new ResponseDTO<MaintenanceDetailedDTO>("Time Slot Not Available");
            // Create Maintenance
            MaintenanceDetailedDTO? maintenance = _maintenanceRepository.CreateNewMaintenance(
                item,
                clerk,
                technician,
                createMaintenanceDTO
            );
            if (maintenance == null)
                return new ResponseDTO<MaintenanceDetailedDTO>("Failed to Create Maintenance");
            return new ResponseDTO<MaintenanceDetailedDTO>(maintenance);
        }

        public ResponseDTO<MaintenanceDetailedDTO> BorrowItemForMaintenance(
            int id,
            int technicianId
        )
        {
            // Check if Technician Exists
            User? technician = _userRepository.GetUserEntityById(technicianId);
            if (technician == null)
                return new ResponseDTO<MaintenanceDetailedDTO>("Technician Not Found");
            // Check if Maintenance Exists
            Maintenance? maintenance = _maintenanceRepository.GetMaintenanceEntityById(id);
            if (maintenance == null)
                return new ResponseDTO<MaintenanceDetailedDTO>("Maintenance Not Found");
            // Check if Maintenance is Scheduled
            if (maintenance.Status != "Scheduled")
                return new ResponseDTO<MaintenanceDetailedDTO>("Maintenance is Not Scheduled");
            // Check if Technician is Assigned
            if (maintenance.TechnicianId != technicianId)
                return new ResponseDTO<MaintenanceDetailedDTO>(
                    "Only Assigned Technician can Borrow Item"
                );
            // Borrow Item for Maintenance
            MaintenanceDetailedDTO? borrowedMaintenance =
                _maintenanceRepository.BorrowItemForMaintenance(maintenance);
            if (borrowedMaintenance == null)
                return new ResponseDTO<MaintenanceDetailedDTO>(
                    "Failed to Borrow Item for Maintenance"
                );
            return new ResponseDTO<MaintenanceDetailedDTO>(borrowedMaintenance);
        }

        public ResponseDTO<MaintenanceDetailedDTO> SubmitMaintenanceUpdate(
            int id,
            int technicianId,
            SubmitMaintenanceDTO submitMaintenanceDTO
        )
        {
            // Check if Technician Exists
            User? technician = _userRepository.GetUserEntityById(technicianId);
            if (technician == null)
                return new ResponseDTO<MaintenanceDetailedDTO>("Technician Not Found");
            // Check if Maintenance Exists
            Maintenance? maintenance = _maintenanceRepository.GetMaintenanceEntityById(id);
            if (maintenance == null)
                return new ResponseDTO<MaintenanceDetailedDTO>("Maintenance Not Found");
            // Check if Maintenance is Ongoing
            if (maintenance.Status != "Ongoing")
                return new ResponseDTO<MaintenanceDetailedDTO>("Maintenance is Not Ongoing");
            // Check if Technician is Assigned
            if (maintenance.TechnicianId != technicianId)
                return new ResponseDTO<MaintenanceDetailedDTO>(
                    "Only Assigned Technician can Update Maintenance"
                );
            // Submit Maintenance Update
            MaintenanceDetailedDTO? updatedMaintenance =
                _maintenanceRepository.SubmitMaintenanceUpdate(maintenance, submitMaintenanceDTO);
            if (updatedMaintenance == null)
                return new ResponseDTO<MaintenanceDetailedDTO>(
                    "Failed to Submit Maintenance Update"
                );
            return new ResponseDTO<MaintenanceDetailedDTO>(updatedMaintenance);
        }

        public ResponseDTO<MaintenanceDetailedDTO> ReviewMaintenance(
            int id,
            int ClerkId,
            ReviewMaintenanceDTO reviewMaintenanceDTO
        )
        {
            // Check if Clerk Exists
            User? clerk = _userRepository.GetUserEntityById(ClerkId);
            if (clerk == null)
                return new ResponseDTO<MaintenanceDetailedDTO>("Clerk Not Found");
            // Check if Maintenance Exists
            Maintenance? maintenance = _maintenanceRepository.GetMaintenanceEntityById(id);
            if (maintenance == null)
                return new ResponseDTO<MaintenanceDetailedDTO>("Maintenance Not Found");
            // Check if Maintenance is Under Review
            if (maintenance.Status != "UnderReview")
                return new ResponseDTO<MaintenanceDetailedDTO>("Maintenance is Not Under Review");
            // Review Maintenance
            MaintenanceDetailedDTO? reviewedMaintenance = _maintenanceRepository.ReviewMaintenance(
                maintenance,
                clerk,
                reviewMaintenanceDTO
            );
            if (reviewedMaintenance == null)
                return new ResponseDTO<MaintenanceDetailedDTO>("Failed to Review Maintenance");
            return new ResponseDTO<MaintenanceDetailedDTO>(reviewedMaintenance);
        }
    }
}
