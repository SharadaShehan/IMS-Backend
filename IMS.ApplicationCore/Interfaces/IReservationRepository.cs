using IMS.ApplicationCore.DTO;
using IMS.ApplicationCore.Model;

namespace IMS.ApplicationCore.Interfaces
{
    public interface IReservationRepository
    {
        ItemReservation? GetReservationEntityById(int id);
        ItemReservationDetailedDTO? GetReservationDTOById(int id);
        List<ItemReservationDTO> GetAllReservationDTOs(int itemId);
        bool CheckTimeSlotAvailability(DateTime startDate, DateTime endDate);
    }
}
