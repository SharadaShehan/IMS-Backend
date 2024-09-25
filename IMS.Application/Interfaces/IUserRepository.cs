using IMS.Application.DTO;
using IMS.Core.Model;

namespace IMS.Application.Interfaces
{
    public interface IUserRepository
    {
        User? GetUserEntityById(int id);
        UserDTO? GetUserDTOById(int id);
        List<UserDTO> GetAllUserDTOs();
        UserDTO? UpdateUserRole(int userId, string roleName);
        List<UserDTO> GetAllTechnicianDTOs();
    }
}
