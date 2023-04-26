using Newtonsoft.Json;

namespace Communications.Requests
{
    public class AddPlantRequest
    {
        [JsonProperty("Name")] public string Name { get; set; }
    }
}