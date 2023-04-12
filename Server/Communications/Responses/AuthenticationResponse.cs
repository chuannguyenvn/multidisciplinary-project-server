using Newtonsoft.Json;

namespace Communications.Responses
{
    public class AuthenticationResponse
    {
        [JsonProperty("Token")] public string Token { get; set; }
    }
}