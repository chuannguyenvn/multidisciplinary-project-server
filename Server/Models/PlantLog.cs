namespace Server.Models;

public class PlantLog
{
    public int Id { get; set; }
    public char Type { get; set; }
    public DateTime Timestamp { get; set; }
    public string ValueString { get; set; }
}