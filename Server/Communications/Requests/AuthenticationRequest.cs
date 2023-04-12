using Newtonsoft.Json;

namespace Communications.Requests
{
    public class AuthenticationRequest
    {
        [JsonProperty("Username")] public string Username { get; set; }
        [JsonProperty("Password")] public string Password { get; set; }
    }
}