using IMS.Application.DTO;
using IMS.Application.Interfaces;
using IMS.Core.Model;
using IMS.Infrastructure.Services;

namespace IMS.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly DataBaseContext _dbContext;

        public UserRepository(DataBaseContext dbContext)
        {
            _dbContext = dbContext;
        }

        public User? GetUserEntityById(int id)
        {
            return _dbContext.users.Where(u => u.UserId == id && u.IsActive).FirstOrDefault();
        }

        public UserDTO? GetUserDTOById(int id)
        {
            return _dbContext
                .users.Where(u => u.UserId == id && u.IsActive)
                .Select(u => new UserDTO
                {
                    userId = u.UserId,
                    email = u.Email,
                    firstName = u.FirstName,
                    lastName = u.LastName,
                    contactNumber = u.ContactNumber,
                    role = u.Role,
                })
                .FirstOrDefault();
        }

        public List<UserDTO> GetAllUserDTOs()
        {
            return _dbContext
                .users.Where(u => u.IsActive)
                .Select(u => new UserDTO
                {
                    userId = u.UserId,
                    email = u.Email,
                    firstName = u.FirstName,
                    lastName = u.LastName,
                    contactNumber = u.ContactNumber,
                    role = u.Role,
                })
                .ToList();
        }

        public UserDTO? UpdateUserRole(int userId, string roleName)
        {
            User? user = _dbContext
                .users.Where(u => u.UserId == userId && u.IsActive)
                .FirstOrDefault();
            if (user == null)
                return null;
            user.Role = roleName;
            _dbContext.Update(user);
            _dbContext.SaveChanges();
            return new UserDTO
            {
                userId = user.UserId,
                email = user.Email,
                firstName = user.FirstName,
                lastName = user.LastName,
                contactNumber = user.ContactNumber,
                role = user.Role,
            };
        }
    }
}
