using Newtonsoft.Json;

namespace Communications.Requests
{
    public class EditPlantRequest
    {
        [JsonProperty("NewName")] public string NewName { get; set; }

        [JsonProperty("NewWateringRuleRepeats")]
        public string NewWateringRuleRepeats { get; set; }

        [JsonProperty("NewWateringRuleMetrics")]
        public string NewWateringRuleMetrics { get; set; }
    }
}