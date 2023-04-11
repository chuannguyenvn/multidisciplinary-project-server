namespace Communications.Responses;

public class PlantDataResponse
{
    public PlantDataRange PlantDataRange { get; set; }
    public List<PlantDataPoint> PlantDataPoints { get; set; }
}

public class PlantDataPoint
{
    public DateTime Timestamp { get; set; }
    public float LightValue { get; set; }
    public float TemperatureValue { get; set; }
    public float MoistureValue { get; set; }
}

public enum PlantDataRange
{
    Latest,
    LastHour,
    Last24Hours,
}