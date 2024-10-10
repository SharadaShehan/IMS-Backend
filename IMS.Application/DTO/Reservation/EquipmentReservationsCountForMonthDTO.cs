namespace IMS.Application.DTO
{
    public class EquipmentReservationsCountForMonthDTO
    {
        public int equipmentId { get; set; }
        public string name { get; set; }
        public string model { get; set; }
        public int count { get; set; }
        public int labId { get; set; }
        public string labName { get; set; }
    }
}
