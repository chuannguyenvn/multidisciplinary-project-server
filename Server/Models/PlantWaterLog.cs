namespace Server.Models;

public class PlantWaterLog
{
    public int Id { get; set; }
    public DateTime Timestamp { get; set; }
    public bool IsManual { get; set; }
    
    public PlantInformation LoggedPlant { get; set; }
}