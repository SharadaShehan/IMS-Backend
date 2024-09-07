using IMS.ApplicationCore.Model;
using IMS.ApplicationCore.Interfaces;
using IMS.ApplicationCore.DTO;

namespace IMS.ApplicationCore.Services
{
    public class MaintenanceService
    {
        private readonly IUserRepository _userRepository;
        private readonly IItemRepository _itemRepository;
        private readonly IMaintenanceRepository _maintenanceRepository;
        private readonly IReservationRepository _reservationRepository;

        public MaintenanceService(IUserRepository userRepository, IItemRepository itemRepository, IMaintenanceRepository maintenanceRepository, IReservationRepository reservationRepository)
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

        public ResponseDTO<MaintenanceDetailedDTO> CreateNewMaintenance(CreateMaintenanceDTO createMaintenanceDTO, int clerkId)
        {
            // Check if Clerk Exists
            User? clerk = _userRepository.GetUserEntityById(clerkId);
            if (clerk == null) return new ResponseDTO<MaintenanceDetailedDTO>("Clerk Not Found");
            // Check if Item Exists
            Item? item = _itemRepository.GetItemEntityById(createMaintenanceDTO.itemId);
            if (item == null) return new ResponseDTO<MaintenanceDetailedDTO>("Item Not Found");
            if (item.Status == "Unavailable") return new ResponseDTO<MaintenanceDetailedDTO>("Item is Unavailable");
            // Check if Technician Exists
            User? technician = _userRepository.GetUserEntityById(createMaintenanceDTO.technicianId);
            if (technician == null) return new ResponseDTO<MaintenanceDetailedDTO>("Technician Not Found");
            // Check if Time Slot is Available
            if (!_maintenanceRepository.CheckTimeSlotAvailability(createMaintenanceDTO.startDate, createMaintenanceDTO.endDate)) return new ResponseDTO<MaintenanceDetailedDTO>("Time Slot Not Available");
            if (!_reservationRepository.CheckTimeSlotAvailability(createMaintenanceDTO.startDate, createMaintenanceDTO.endDate)) return new ResponseDTO<MaintenanceDetailedDTO>("Time Slot Not Available");
            // Create Maintenance
            MaintenanceDetailedDTO? maintenance = _maintenanceRepository.CreateNewMaintenance(createMaintenanceDTO, item, clerk, technician);
            if (maintenance == null) return new ResponseDTO<MaintenanceDetailedDTO>("Failed to Create Maintenance");
            return new ResponseDTO<MaintenanceDetailedDTO>(maintenance);
        }

        public ResponseDTO<MaintenanceDetailedDTO> ReviewMaintenance(int id, ReviewMaintenanceDTO reviewMaintenanceDTO, int ClerkId, bool accepted)
        {
            // Check if Clerk Exists
            User? clerk = _userRepository.GetUserEntityById(ClerkId);
            if (clerk == null) return new ResponseDTO<MaintenanceDetailedDTO>("Clerk Not Found");
            // Check if Maintenance Exists
            Maintenance? maintenance = _maintenanceRepository.GetMaintenanceEntityById(id);
            if (maintenance == null) return new ResponseDTO<MaintenanceDetailedDTO>("Maintenance Not Found");
            // Check if Maintenance is Under Review
            if (maintenance.Status != "UnderReview") return new ResponseDTO<MaintenanceDetailedDTO>("Maintenance is Not Under Review");
            // Review Maintenance
            MaintenanceDetailedDTO? reviewedMaintenance = _maintenanceRepository.ReviewMaintenance(maintenance, reviewMaintenanceDTO, clerk, accepted);
            if (reviewedMaintenance == null) return new ResponseDTO<MaintenanceDetailedDTO>("Failed to Review Maintenance");
            return new ResponseDTO<MaintenanceDetailedDTO>(reviewedMaintenance);
        }



    }
}
