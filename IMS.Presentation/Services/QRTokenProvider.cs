using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace IMS.Presentation.Services
{
    public interface IQRTokenProvider
    {
        public Task<string?> getQRToken(int reservationId, int userId);
        public Task<DecodedQRToken> validateQRToken(string token);
    }

    public class QRTokenProvider : IQRTokenProvider
    {
        private readonly string qRTokenSecret;

        public QRTokenProvider(IConfiguration configuration)
        {
            this.qRTokenSecret = configuration.GetSection("QRToken")["Secret"];
        }

        public async Task<string?> getQRToken(int reservationId, int userId)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(qRTokenSecret);
                var claims = new[]
                {
                    new Claim("UserId" as string, userId.ToString() as string),
                    new Claim("ReservationId" as string, reservationId.ToString() as string),
                };
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(claims),
                    Expires = DateTime.UtcNow.AddMinutes(2),
                    SigningCredentials = new SigningCredentials(
                        new SymmetricSecurityKey(key),
                        SecurityAlgorithms.HmacSha256Signature
                    ),
                };
                // Create token
                var token = tokenHandler.CreateToken(tokenDescriptor);
                // Return the generated token
                return tokenHandler.WriteToken(token);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return null;
            }
        }

        public async Task<DecodedQRToken> validateQRToken(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                if (tokenHandler.CanReadToken(token))
                {
                    var key = Encoding.UTF8.GetBytes(qRTokenSecret);
                    // Set the token validation parameters
                    var validationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = false,
                        ClockSkew =
                            TimeSpan.Zero // Remove default clock skew of 5 mins
                        ,
                    };

                    // Validate the token and extract the claims
                    var principal = tokenHandler.ValidateToken(
                        token,
                        validationParameters,
                        out SecurityToken validatedToken
                    );
                    var claims = principal.Claims;
                    // Get the userId from the claims
                    var userId = claims.FirstOrDefault(x => x.Type == "UserId")?.Value;
                    // Get the reservationId from the claims
                    var reservationId = claims
                        .FirstOrDefault(x => x.Type == "ReservationId")
                        ?.Value;

                    // Check if the claims are not null
                    if (userId != null && reservationId != null)
                    {
                        // Return the decoded token
                        return new DecodedQRToken(int.Parse(userId), int.Parse(reservationId));
                    }
                    else
                    {
                        throw new Exception("Invalid Token Format");
                    }
                }
                else
                {
                    throw new Exception("Invalid Token Format");
                }
            }
            catch (SecurityTokenException ex)
            {
                Debug.WriteLine(ex);
                return new DecodedQRToken(ex.Message);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return new DecodedQRToken(ex.Message);
            }
        }
    }

    public class DecodedQRToken
    {
        public bool success { get; set; }
        public int? userId { get; set; }
        public int? reservationId { get; set; }
        public string? message { get; set; }

        public DecodedQRToken(int userId, int reservationId)
        {
            this.success = true;
            this.userId = userId;
            this.reservationId = reservationId;
        }

        public DecodedQRToken(string message)
        {
            this.success = false;
            this.message = message;
        }
    }
}
