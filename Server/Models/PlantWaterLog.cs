namespace Server.Models;

public class PlantWaterLog
{
    public int Id { get; set; }
    public PlantInformation WateredPlant { get; set; }
    public bool IsManual { get; set; }
}