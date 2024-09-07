using IMS.ApplicationCore.Model;
using IMS.ApplicationCore.Interfaces;
using IMS.ApplicationCore.DTO;

namespace IMS.ApplicationCore.Services
{
    public class ReservationService
    {
        private readonly IReservationRepository _reservationRepository;

        public ReservationService(IReservationRepository reservationRepository)
        {
            _reservationRepository = reservationRepository;
        }

        public ItemReservationDetailedDTO? GetReservationById(int id)
        {
            return _reservationRepository.GetReservationDTOById(id);
        }

        public List<ItemReservationDTO> GetAllReservations(int itemId)
        {
            return _reservationRepository.GetAllReservationDTOs(itemId);
        }

    }
}
