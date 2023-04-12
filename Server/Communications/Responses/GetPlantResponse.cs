using Newtonsoft.Json;

namespace Communications.Responses
{
    public class GetPlantResponse
    {
        [JsonProperty("PlantInformations")] public List<GetPlantResponseUnit> PlantInformations { get; set; }
    }

    public class GetPlantResponseUnit
    {
        [JsonProperty("Id")] public int Id { get; set; }
        [JsonProperty("Name")] public string Name { get; set; }
        [JsonProperty("CreatedDate")] public DateTime CreatedDate { get; set; }
        [JsonProperty("Photo")] public string Photo { get; set; }
        [JsonProperty("RecognizerCode")] public string RecognizerCode { get; set; }
    }
}