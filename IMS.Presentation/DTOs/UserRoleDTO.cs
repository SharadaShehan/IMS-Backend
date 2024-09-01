namespace IMS.Presentation.DTOs
{
    public class UserRoleDTO
    {
        public string RoleName { get; set; }
        public UserRoleDTO(string roleName) { RoleName = roleName; }
    }
}
