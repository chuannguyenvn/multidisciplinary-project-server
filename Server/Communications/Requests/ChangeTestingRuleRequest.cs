using Newtonsoft.Json;

namespace Communications.Requests
{
    public class ChangeTestingRuleRequest
    {
        [JsonProperty("NewRule")] public string NewRule { get; set; }
    }
}