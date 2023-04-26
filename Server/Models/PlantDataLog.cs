namespace Server.Models;

public class PlantDataLog
{
    public int Id { get; set; }
    public DateTime Timestamp { get; set; }
    public float LightValue { get; set; }
    public float TemperatureValue { get; set; }
    public float MoistureValue { get; set; }
    
    public PlantInformation LoggedPlant { get; set; }
}