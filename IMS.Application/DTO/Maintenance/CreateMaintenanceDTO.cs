namespace IMS.Application.DTO
{
    public class CreateMaintenanceDTO
    {
        public int itemId { get; set; }
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }
        public int technicianId { get; set; }
        public string taskDescription { get; set; }
    }
}
