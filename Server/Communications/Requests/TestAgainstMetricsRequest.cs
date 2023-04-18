using Newtonsoft.Json;

namespace Communications.Requests
{
    public class TestAgainstMetricsRequest
    {
        [JsonProperty("Light")] public float Light { get; set; }
        [JsonProperty("Temperature")] public float Temperature { get; set; }
        [JsonProperty("Moisture")] public float Moisture { get; set; }
    }
}