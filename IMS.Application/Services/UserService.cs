using IMS.Core.Model;
using IMS.Application.Interfaces;
using IMS.Application.DTO;

namespace IMS.Application.Services
{
    public class UserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public UserDTO? GetUserById(int id)
        {
            return _userRepository.GetUserDTOById(id);
        }

        public List<UserDTO> GetAllUsers()
        {
            return _userRepository.GetAllUserDTOs();
        }

        public ResponseDTO<UserDTO> UpdateUserRole(int userId, string roleName)
        {
            UserDTO? userDTO = _userRepository.UpdateUserRole(userId, roleName);
            if (userDTO == null) return new ResponseDTO<UserDTO>("User Not Found");
            return new ResponseDTO<UserDTO>(userDTO);
        }

    }
}
