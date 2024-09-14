using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace IMS.Presentation.Services
{
    public interface IQRTokenProvider
    {
        public Task<string?> getQRToken(int eventId, int userId, bool isReservation);
        public Task<DecodedQRToken> validateQRToken(string token);
    }

    public class QRTokenProvider : IQRTokenProvider
    {
        private readonly string qRTokenSecret;

        public QRTokenProvider(IConfiguration configuration)
        {
            this.qRTokenSecret = configuration.GetSection("QRToken")["Secret"];
        }

        public async Task<string?> getQRToken(int eventId, int userId, bool isReservation)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(qRTokenSecret);
                var claims = new[] { new Claim("UserId" as string, userId.ToString() as string) };
                if (isReservation)
                {
                    claims.Append(
                        new Claim("ReservationId" as string, eventId.ToString() as string)
                    );
                }
                else
                {
                    claims.Append(
                        new Claim("MaintenanceId" as string, eventId.ToString() as string)
                    );
                }
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
                    var key = Encoding.ASCII.GetBytes(qRTokenSecret);
                    // Set the token validation parameters
                    var validationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = true,
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
                    // Get the eventId from the claims
                    var eventId = claims.FirstOrDefault(x => x.Type == "EventId")?.Value;
                    // Get the isReservation from the claims
                    var isReservation = claims
                        .FirstOrDefault(x => x.Type == "IsReservation")
                        ?.Value;

                    // Check if the claims are not null
                    if (userId != null && eventId != null && isReservation != null)
                    {
                        // Return the decoded token
                        return new DecodedQRToken(
                            int.Parse(eventId),
                            int.Parse(userId),
                            bool.Parse(isReservation)
                        );
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
        public int? eventId { get; set; }
        public bool? isReservation { get; set; }
        public string? message { get; set; }

        public DecodedQRToken(int eventId, int userId, bool isReservation)
        {
            this.success = true;
            this.userId = userId;
            this.eventId = eventId;
            this.isReservation = isReservation;
        }

        public DecodedQRToken(string message)
        {
            this.success = false;
            this.message = message;
        }
    }
}
