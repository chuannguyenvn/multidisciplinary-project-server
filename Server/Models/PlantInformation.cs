namespace Server.Models;

public class PlantInformation
{
    public int Id { get; set; }
    public string Name { get; set; }
    public DateTime CreatedDate { get; set; }
    public string Photo { get; set; }
    public string RecognizerCode { get; set; }

    public User Owner { get; set; }
    public List<PlantLog> PlantLogs { get; set; }
}