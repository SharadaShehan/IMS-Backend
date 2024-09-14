namespace IMS.Application.DTO
{
    public class UserRoleDTO
    {
        public string role { get; set; }

        public UserRoleDTO(string roleName)
        {
            role = roleName;
        }
    }
}
