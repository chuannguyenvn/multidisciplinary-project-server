using Newtonsoft.Json;

namespace Communications.Requests
{
    public class EditPlantRequest
    {
        [JsonProperty("NewName")] public string NewName { get; set; }
        [JsonProperty("NewPhoto")] public string NewPhoto { get; set; }
        [JsonProperty("NewWateringRule")] public string NewWateringRule { get; set; }
    }
}