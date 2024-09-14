using System.Diagnostics;
using System.Text.Json;

namespace IMS.Infrastructure.Services
{
    public interface IAuthServerContext
    {
        public Task<List<AuthUserDTO>?> PollUserData();
    }

    public class AuthServerContext : IAuthServerContext
    {
        private string Endpoint { get; set; }
        private string ClientId { get; set; }
        private string ClientSecret { get; set; }
        private HttpClient httpClient { get; set; }

        public AuthServerContext(AuthServerContextOptions options)
        {
            this.Endpoint = options.Endpoint;
            this.ClientId = options.ClientId;
            this.ClientSecret = options.ClientSecret;
            this.httpClient = new HttpClient();
        }

        public async Task<List<AuthUserDTO>?> PollUserData()
        {
            try
            {
                // format to send credentials in request body
                var data = new Dictionary<string, string>
                {
                    { "client_id", ClientId },
                    { "client_secret", ClientSecret },
                };
                var content = new FormUrlEncodedContent(data);
                // send POST request to polling endpoint of auth server
                var response = await httpClient.PostAsync(this.Endpoint + "/poll-data", content);
                var responseString = await response.Content.ReadAsStringAsync();

                // if response is successful, deserialize the response to list of DBUserDTO
                if (response.IsSuccessStatusCode)
                {
                    List<AuthUserDTO> userDTOs =
                        JsonSerializer.Deserialize<List<AuthUserDTO>>(responseString)
                        as List<AuthUserDTO>;
                    return userDTOs;
                }
                // if response is not successful, return null
                return null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return null;
            }
        }
    }

    public class AuthServerContextOptions
    {
        public string Endpoint { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
    }

    public class AuthUserDTO
    {
        public string email { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string contactNumber { get; set; }
    }
}
