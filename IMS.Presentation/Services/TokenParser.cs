using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using IMS.Application.DTO;
using IMS.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

namespace IMS.Presentation.Services
{
    public interface ITokenParser
    {
        public Task<UserDTO?> getUser(string? authorizationHeader);
    }

    public class TokenParser : ITokenParser
    {
        public DataBaseContext _dbContext;

        public TokenParser(DataBaseContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<UserDTO?> getUser(string? authorizationHeader)
        {
            try
            {
                if (authorizationHeader != null && authorizationHeader.StartsWith("Bearer "))
                {
                    // Extract the token by removing the "Bearer " prefix
                    var token = authorizationHeader.Substring("Bearer ".Length).Trim();
                    // Decode the JWT token
                    var handler = new JwtSecurityTokenHandler();
                    var jwtToken = handler.ReadJwtToken(token);
                    // Extract the email claim from the token
                    var emailClaim = jwtToken.Claims.FirstOrDefault(claim => claim.Type == "email");
                    if (emailClaim == null)
                        throw new Exception("Email not found in JWT token.");
                    // Query the database for the user with this email
                    UserDTO? user = await _dbContext
                        .users.Where(dbUser => dbUser.IsActive)
                        .Select(dbUser => new UserDTO
                        {
                            userId = dbUser.UserId,
                            email = dbUser.Email,
                            firstName = dbUser.FirstName,
                            lastName = dbUser.LastName,
                            contactNumber = dbUser.ContactNumber,
                            role = dbUser.Role,
                        })
                        .FirstOrDefaultAsync(u => u.email == emailClaim.Value);
                    if (user == null)
                        throw new Exception("User not found");
                    return user;
                }
                else
                    return null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return null;
            }
        }
    }
}
