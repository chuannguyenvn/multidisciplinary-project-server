using Newtonsoft.Json;

namespace Communications.Responses
{
    public class AddPlantResponse
    {
        [JsonProperty("RecognizerCode")] public string RecognizerCode { get; set; }
    }
}