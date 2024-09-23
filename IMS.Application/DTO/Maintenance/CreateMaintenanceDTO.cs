namespace IMS.Application.DTO
{
    public class CreateMaintenanceDTO
    {
        public int itemId { get; set; }
        public string startDate { get; set; }
        public string endDate { get; set; }
        public int technicianId { get; set; }
        public string taskDescription { get; set; }
    }
}
