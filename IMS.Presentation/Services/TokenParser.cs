using IMS.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using IMS.ApplicationCore.Model;
using System.Diagnostics;

namespace IMS.Presentation.Services
{
    public interface ITokenParser
    {
        public Task<User?> getUser(string? authorizationHeader);
    }

    public class TokenParser : ITokenParser 
    {
        public DataBaseContext _dbContext;
        public TokenParser(DataBaseContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<User?> getUser(string? authorizationHeader)
        {
            try {
                if (authorizationHeader != null && authorizationHeader.StartsWith("Bearer ")) {
                    // Extract the token by removing the "Bearer " prefix
                    var token = authorizationHeader.Substring("Bearer ".Length).Trim();
                    // Decode the JWT token
                    var handler = new JwtSecurityTokenHandler();
                    var jwtToken = handler.ReadJwtToken(token);
                    // Extract the email claim from the token
                    var emailClaim = jwtToken.Claims.FirstOrDefault(claim => claim.Type == "email");
                    if (emailClaim == null) throw new Exception("Email not found in JWT token.");
                    // Query the database for the user with this email
                    User? user = await _dbContext.users.FirstOrDefaultAsync(u => u.Email == emailClaim.Value);
                    if (user == null) throw new Exception("User not found");
                    return user;
                }
                else return null;
            } catch (Exception ex) {
                Debug.WriteLine(ex);
                return null;
            }
        }
    }
}
