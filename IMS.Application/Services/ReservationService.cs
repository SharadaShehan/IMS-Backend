using IMS.Application.DTO;
using IMS.Application.Interfaces;
using IMS.Core.Model;

namespace IMS.Application.Services
{
    public class ReservationService
    {
        private readonly IReservationRepository _reservationRepository;
        private readonly IUserRepository _userRepository;
        private readonly IEquipmentRepository _equipmentRepository;
        private readonly IItemRepository _itemRepository;
        private readonly IMaintenanceRepository _maintenanceRepository;

        public ReservationService(
            IReservationRepository reservationRepository,
            IUserRepository userRepository,
            IEquipmentRepository equipmentRepository,
            IItemRepository itemRepository,
            IMaintenanceRepository maintenanceRepository
        )
        {
            _reservationRepository = reservationRepository;
            _userRepository = userRepository;
            _equipmentRepository = equipmentRepository;
            _maintenanceRepository = maintenanceRepository;
            _itemRepository = itemRepository;
        }

        public ItemReservationDetailedDTO? GetReservationById(int id)
        {
            return _reservationRepository.GetReservationDTOById(id);
        }

        public List<ItemReservationDTO> GetAllReservations(int itemId)
        {
            return _reservationRepository.GetAllReservationDTOs(itemId);
        }

        public List<ItemReservationDTO> GetAllReservations(
            bool requested,
            bool reserved,
            bool borrowed
        )
        {
            if (requested)
            {
                return _reservationRepository.GetAllRequestedReservationDTOs();
            }
            else if (reserved)
            {
                return _reservationRepository.GetAllReservedReservationDTOs();
            }
            else if (borrowed)
            {
                return _reservationRepository.GetAllBorrowedReservationDTOs();
            }
            else
            {
                return _reservationRepository.GetAllNonCompletedReservationDTOs();
            }
        }

        public List<ItemReservationDTO> GetAllReservationsByStudent(int studentId, bool borrowed)
        {
            if (borrowed)
            {
                return _reservationRepository.GetAllBorrowedReservationDTOsByStudent(studentId);
            }
            else
            {
                return _reservationRepository.GetAllNonBorrowedReservationDTOsByStudent(studentId);
            }
        }

        public ResponseDTO<ItemReservationDetailedDTO> CreateNewReservation(
            int studentId,
            RequestEquipmentDTO requestEquipmentDTO
        )
        {
            // Check if Student Exists
            User? student = _userRepository.GetUserEntityById(studentId);
            if (student == null)
                return new ResponseDTO<ItemReservationDetailedDTO>("Student Not Found");
            if (student.Role != "Student" && student.Role != "AcademicStaff")
                return new ResponseDTO<ItemReservationDetailedDTO>(
                    "Only Students/Academic Staff can make reservation requests"
                );
            // Check if Equipment Exists
            Equipment? equipment = _equipmentRepository.GetEquipmentEntityById(
                requestEquipmentDTO.equipmentId
            );
            if (equipment == null)
                return new ResponseDTO<ItemReservationDetailedDTO>("Equipment Not Found");
            // Check if Time Slot is Available
            if (
                !_reservationRepository.CheckTimeSlotAvailability(
                    requestEquipmentDTO.startDate,
                    requestEquipmentDTO.endDate
                )
            )
                return new ResponseDTO<ItemReservationDetailedDTO>("Time Slot is Unavailable");
            if (
                !_maintenanceRepository.CheckTimeSlotAvailability(
                    requestEquipmentDTO.startDate,
                    requestEquipmentDTO.endDate
                )
            )
                return new ResponseDTO<ItemReservationDetailedDTO>("Time Slot is Unavailable");
            // Create Reservation
            ItemReservationDetailedDTO? reservation =
                _reservationRepository.RequestEquipmentReservation(
                    equipment,
                    student,
                    requestEquipmentDTO
                );
            if (reservation == null)
                return new ResponseDTO<ItemReservationDetailedDTO>("Failed to Create Reservation");
            return new ResponseDTO<ItemReservationDetailedDTO>(reservation);
        }

        public ResponseDTO<ItemReservationDetailedDTO> RespondToReservationRequest(
            int reservationId,
            int clerkId,
            RespondReservationDTO respondReservationDTO
        )
        {
            // Check if Clerk Exists
            User? clerk = _userRepository.GetUserEntityById(clerkId);
            if (clerk == null)
                return new ResponseDTO<ItemReservationDetailedDTO>("Clerk Not Found");
            if (clerk.Role != "Clerk")
                return new ResponseDTO<ItemReservationDetailedDTO>(
                    "Only Clerks can accept reservations"
                );
            // Check if Reservation Exists
            ItemReservation? reservation = _reservationRepository.GetReservationEntityById(
                reservationId
            );
            if (reservation == null)
                return new ResponseDTO<ItemReservationDetailedDTO>("Reservation Not Found");
            // Check if Reservation is Pending
            if (reservation.Status != "Pending")
                return new ResponseDTO<ItemReservationDetailedDTO>("Reservation is Not Pending");
            // If Clerk Accepts Reservation
            if (respondReservationDTO.accepted)
            {
                // Check if Item Exists
                if (respondReservationDTO.itemId == null)
                    return new ResponseDTO<ItemReservationDetailedDTO>("Item Id not Found");
                Item? item = _itemRepository.GetItemEntityById(respondReservationDTO.itemId.Value);
                if (item == null)
                    return new ResponseDTO<ItemReservationDetailedDTO>("Item Not Found");
                // Check if Time Slot is Available
                if (
                    !_reservationRepository.CheckTimeSlotAvailability(
                        reservation.StartDate,
                        reservation.EndDate
                    )
                )
                    return new ResponseDTO<ItemReservationDetailedDTO>("Time Slot is Unavailable");
                if (
                    !_maintenanceRepository.CheckTimeSlotAvailability(
                        reservation.StartDate,
                        reservation.EndDate
                    )
                )
                    return new ResponseDTO<ItemReservationDetailedDTO>("Time Slot is Unavailable");
                // Accept Reservation
                ItemReservationDetailedDTO? acceptedReservation =
                    _reservationRepository.AcceptEquipmentReservation(reservation, item, clerk);
                if (acceptedReservation == null)
                    return new ResponseDTO<ItemReservationDetailedDTO>(
                        "Failed to Accept Reservation"
                    );
                return new ResponseDTO<ItemReservationDetailedDTO>(acceptedReservation);
            }
            // If Clerk Rejects Reservation
            else
            {
                // Reject Reservation
                ItemReservationDetailedDTO? rejectedReservation =
                    _reservationRepository.RejectEquipmentReservation(
                        reservation,
                        clerk,
                        respondReservationDTO
                    );
                if (rejectedReservation == null)
                    return new ResponseDTO<ItemReservationDetailedDTO>(
                        "Failed to Reject Reservation"
                    );
                return new ResponseDTO<ItemReservationDetailedDTO>(rejectedReservation);
            }
        }

        public ResponseDTO<ItemReservationDetailedDTO> BorrowReservedItem(
            int reservationId,
            int clerkId
        )
        {
            // Check if Clerk Exists
            User? clerk = _userRepository.GetUserEntityById(clerkId);
            if (clerk == null)
                return new ResponseDTO<ItemReservationDetailedDTO>("Clerk Not Found");
            if (clerk.Role != "Clerk")
                return new ResponseDTO<ItemReservationDetailedDTO>("Only Clerks can borrow items");
            // Check if Reservation Exists
            ItemReservation? reservation = _reservationRepository.GetReservationEntityById(
                reservationId
            );
            if (reservation == null)
                return new ResponseDTO<ItemReservationDetailedDTO>("Reservation Not Found");
            // Check if Reservation is Reserved
            if (reservation.Status != "Reserved")
                return new ResponseDTO<ItemReservationDetailedDTO>("Item is Not Reserved");
            // Borrow Reserved Item
            ItemReservationDetailedDTO? borrowedReservation =
                _reservationRepository.BorrowReservedItem(reservation, clerk);
            if (borrowedReservation == null)
                return new ResponseDTO<ItemReservationDetailedDTO>(
                    "Failed to Update Reserved Item to Borrowed"
                );
            return new ResponseDTO<ItemReservationDetailedDTO>(borrowedReservation);
        }

        public ResponseDTO<ItemReservationDetailedDTO> CancelReservation(
            int reservationId,
            int studentId
        )
        {
            // Check if Reservation Exists
            ItemReservation? reservation = _reservationRepository.GetReservationEntityById(
                reservationId
            );
            if (reservation == null)
                return new ResponseDTO<ItemReservationDetailedDTO>("Reservation Not Found");
            // Check if Student is the Reservation Owner
            if (reservation.ReservedUserId != studentId)
                return new ResponseDTO<ItemReservationDetailedDTO>(
                    "Only Reservation Owner can Cancel Reservation"
                );
            // Check if Reservation is Borrowed
            if (reservation.Status == "Borrowed")
                return new ResponseDTO<ItemReservationDetailedDTO>("Item is already Borrowed");
            // Cancel Reservation
            ItemReservationDetailedDTO? cancelledReservation =
                _reservationRepository.CancelReservation(reservation);
            if (cancelledReservation == null)
                return new ResponseDTO<ItemReservationDetailedDTO>("Failed to Cancel Reservation");
            return new ResponseDTO<ItemReservationDetailedDTO>(cancelledReservation);
        }

        public ResponseDTO<ItemReservationDetailedDTO> ReturnBorrowedItem(
            int reservationId,
            int clerkId
        )
        {
            // Check if Clerk Exists
            User? clerk = _userRepository.GetUserEntityById(clerkId);
            if (clerk == null)
                return new ResponseDTO<ItemReservationDetailedDTO>("Clerk Not Found");
            if (clerk.Role != "Clerk")
                return new ResponseDTO<ItemReservationDetailedDTO>("Only Clerks can return items");
            // Check if Reservation Exists
            ItemReservation? reservation = _reservationRepository.GetReservationEntityById(
                reservationId
            );
            if (reservation == null)
                return new ResponseDTO<ItemReservationDetailedDTO>("Reservation Not Found");
            // Check if Reservation is Borrowed
            if (reservation.Status != "Borrowed")
                return new ResponseDTO<ItemReservationDetailedDTO>("Item is Not Borrowed");
            // Return Borrowed Item
            ItemReservationDetailedDTO? returnedReservation =
                _reservationRepository.ReturnBorrowedItem(reservation, clerk);
            if (returnedReservation == null)
                return new ResponseDTO<ItemReservationDetailedDTO>(
                    "Failed to Update Borrowed Item to Returned"
                );
            return new ResponseDTO<ItemReservationDetailedDTO>(returnedReservation);
        }
    }
}
