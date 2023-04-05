namespace Server.Models;

public class PlantLog
{
    public int Id { get; set; }
    public DateTime Timestamp { get; set; }
    public float LightValue { get; set; }
    public float TemperatureValue { get; set; }
    public float MoistureValue { get; set; }
    
    public PlantInformation PlantInformation { get; set; }
}