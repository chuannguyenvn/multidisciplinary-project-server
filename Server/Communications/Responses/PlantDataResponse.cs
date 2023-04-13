using Newtonsoft.Json;

namespace Communications.Responses
{
    public class PlantDataResponse
    {
        [JsonProperty("PlantDataRange")] public PlantDataRange PlantDataRange { get; set; }
        [JsonProperty("PlantDataPoints")] public List<PlantDataPoint> PlantDataPoints { get; set; }
        [JsonProperty("PlantWaterPoints")] public List<PlantWaterPoint> PlantWaterPoints { get; set; }
    }

    public class PlantDataPoint
    {
        [JsonProperty("Timestamp")] public DateTime Timestamp { get; set; }
        [JsonProperty("LightValue")] public double LightValue { get; set; }
        [JsonProperty("TemperatureValue")] public double TemperatureValue { get; set; }
        [JsonProperty("MoistureValue")] public double MoistureValue { get; set; }
    }

    public class PlantWaterPoint
    {
        [JsonProperty("Timestamp")] public DateTime Timestamp { get; set; }
        [JsonProperty("IsManual")] public bool IsManual { get; set; }
    }

    public enum PlantDataRange
    {
        Latest,
        LastHour,
        Last24Hours,
    }
}