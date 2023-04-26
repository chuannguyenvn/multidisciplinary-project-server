namespace Server.Models;

public class PlantInformation
{
    public int Id { get; set; }
    public string Name { get; set; }
    public DateTime CreatedDate { get; set; }
    public string RecognizerCode { get; set; }
    public string WateringRuleRepeats { get; set; }
    public string WateringRuleMetrics { get; set; }

    public User Owner { get; set; }
}