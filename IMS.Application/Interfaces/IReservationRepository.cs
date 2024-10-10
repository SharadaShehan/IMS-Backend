﻿using IMS.Application.DTO;
using IMS.Core.Model;

namespace IMS.Application.Interfaces
{
    public interface IReservationRepository
    {
        ItemReservation? GetReservationEntityById(int id);
        ItemReservationDetailedDTO? GetReservationDTOById(int id);
        List<ItemReservationDTO> GetAllReservationDTOs(int itemId);
        List<ItemReservationDTO> GetAllNonCompletedReservationDTOs();
        List<ItemReservationDTO> GetAllRequestedReservationDTOs();
        List<ItemReservationDTO> GetAllReservedReservationDTOs();
        List<ItemReservationDTO> GetAllBorrowedReservationDTOs();
        List<ItemReservationDTO> GetAllBorrowedReservationDTOsByStudent(int studentId);
        List<ItemReservationDTO> GetAllNonBorrowedReservationDTOsByStudent(int studentId);
        ItemReservationDetailedDTO? RequestEquipmentReservation(
            Equipment equipment,
            User student,
            RequestEquipmentDTO requestEquipmentDTO
        );
        ItemReservationDetailedDTO? AcceptEquipmentReservation(
            ItemReservation reservation,
            Item item,
            User clerk
        );
        ItemReservationDetailedDTO? RejectEquipmentReservation(
            ItemReservation reservation,
            User clerk,
            RespondReservationDTO respondReservationDTO
        );
        ItemReservationDetailedDTO? BorrowReservedItem(
            ItemReservation reservation,
            Item item,
            User clerk
        );
        ItemReservationDetailedDTO? CancelReservation(ItemReservation reservation);
        ItemReservationDetailedDTO? ReturnBorrowedItem(
            ItemReservation reservation,
            Item item,
            User clerk
        );
        bool CheckTimeSlotAvailability(DateTime startDate, DateTime endDate);
        List<DueItemReservationDTO> GetAllDueItemReservationDTOs();
        List<DueItemReservationDTO> GetAllPickupPendingReservationDTOs();
        List<EquipmentReservationsCountForMonthDTO> GetReservationsCountForMonth(int year, int month);
        List<EquipmentReservationsCountByMonthDTO> GetReservationsCountByMonth(int equipmentId);
    }
}