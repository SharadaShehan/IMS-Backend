using IMS.ApplicationCore.DTO;
using IMS.ApplicationCore.Interfaces;
using IMS.ApplicationCore.Model;
using IMS.Infrastructure.Services;

namespace IMS.Infrastructure.Repositories
{
    public class ReservationRepository : IReservationRepository
    {
        private readonly DataBaseContext _dbContext;

        public ReservationRepository(DataBaseContext dbContext)
        {
            _dbContext = dbContext;
        }

        public ItemReservation? GetReservationEntityById(int id)
        {
            return _dbContext.itemReservations.Where(rsv => rsv.ItemReservationId == id && rsv.IsActive).FirstOrDefault();
        }

        public ItemReservationDetailedDTO? GetReservationDTOById(int id)
        {
            return _dbContext.itemReservations.Where(rsv => rsv.ItemReservationId == id && rsv.IsActive).Select(rsv => new ItemReservationDetailedDTO
            {
                reservationId = rsv.ItemReservationId,
                equipmentId = rsv.EquipmentId,
                itemName = rsv.Equipment.Name,
                itemModel = rsv.Equipment.Model,
                imageUrl = rsv.Equipment.ImageURL,
                itemId = rsv.ItemId,
                itemSerialNumber = rsv.Item != null ? rsv.Item.SerialNumber : null,
                labId = rsv.Equipment.LabId,
                labName = rsv.Equipment.Lab.LabName,
                startDate = rsv.StartDate,
                endDate = rsv.EndDate,
                reservedUserId = rsv.ReservedUserId,
                reservedUserName = rsv.ReservedUser.FirstName + " " + rsv.ReservedUser.LastName,
                createdAt = rsv.CreatedAt,
                respondedClerkId = rsv.RespondedClerkId,
                respondedClerkName = rsv.RespondedClerk != null ? rsv.RespondedClerk.FirstName + " " + rsv.RespondedClerk.LastName : null,
                responseNote = rsv.ResponseNote,
                respondedAt = rsv.RespondedAt,
                lentClerkId = rsv.LentClerkId,
                lentClerkName = rsv.LentClerk != null ? rsv.LentClerk.FirstName + " " + rsv.LentClerk.LastName : null,
                borrowedAt = rsv.BorrowedAt,
                returnAcceptedClerkId = rsv.ReturnAcceptedClerkId,
                returnAcceptedClerkName = rsv.ReturnAcceptedClerk != null ? rsv.ReturnAcceptedClerk.FirstName + " " + rsv.ReturnAcceptedClerk.LastName : null,
                returnedAt = rsv.ReturnedAt,
                cancelledAt = rsv.CancelledAt,
                status = rsv.Status
            }).FirstOrDefault();
        }

        public List<ItemReservationDTO> GetAllReservationDTOs(int itemId)
        {
            return _dbContext.itemReservations.Where(rsv => rsv.IsActive && rsv.ItemId == itemId).Select(rsv => new ItemReservationDTO
            {
                reservationId = rsv.ItemReservationId,
                equipmentId = rsv.EquipmentId,
                itemName = rsv.Equipment.Name,
                itemModel = rsv.Equipment.Model,
                imageUrl = rsv.Equipment.ImageURL,
                itemId = rsv.ItemId,
                itemSerialNumber = rsv.Item != null ? rsv.Item.SerialNumber : null,
                labId = rsv.Equipment.LabId,
                labName = rsv.Equipment.Lab.LabName,
                startDate = rsv.StartDate,
                endDate = rsv.EndDate,
                reservedUserId = rsv.ReservedUserId,
                reservedUserName = rsv.ReservedUser.FirstName + " " + rsv.ReservedUser.LastName,
                createdAt = rsv.CreatedAt,
                respondedAt = rsv.RespondedAt,
                borrowedAt = rsv.BorrowedAt,
                returnedAt = rsv.ReturnedAt,
                cancelledAt = rsv.CancelledAt,
                status = rsv.Status
            }).OrderByDescending(rsv => rsv.startDate).ToList();
        }

        public bool CheckTimeSlotAvailability(DateTime startDate, DateTime endDate)
        {
            return !_dbContext.itemReservations.Any(rsv => rsv.EndDate >= startDate && rsv.StartDate <= endDate && rsv.IsActive && (rsv.Status == "Reserved" || rsv.Status == "Borrowed"));
        }

    }
}
