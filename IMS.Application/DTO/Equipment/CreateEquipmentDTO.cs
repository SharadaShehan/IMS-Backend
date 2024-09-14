namespace IMS.Application.DTO
{
    public class CreateEquipmentDTO
    {
        public string name { get; set; }
        public string model { get; set; }
        public int labId { get; set; }
        public string? imageURL { get; set; }
        public string? specification { get; set; }
        public int? maintenanceIntervalDays { get; set; }
    }
}
