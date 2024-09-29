namespace IMS.Presentation.Utils
{
    public class AuditData
    {
        public required string userRole;
        public required int userId;
        public required string action;
        public required string objectName;
        public required int objectId;
        public required string status;
    }
}
