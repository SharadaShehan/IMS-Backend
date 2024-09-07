using IMS.ApplicationCore.DTO;
using IMS.ApplicationCore.Model;

namespace IMS.ApplicationCore.Interfaces
{
    public interface IUserRepository
    {
        User? GetUserEntityById(int id);
        UserDTO? GetUserDTOById(int id);
        List<UserDTO> GetAllUserDTOs();
        UserDTO? UpdateUserRole(int userId, string roleName);
    }
}
