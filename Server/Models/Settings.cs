namespace Server.Models;

public class Settings
{
    public string BearerKey { get; set; }
    public string AdafruitLoggingCron { get; set; }
    public string AdafruitUsername { get; set; }
    public string AdafruitKey { get; set; }
    public string AdafruitFeedName { get; set; }
    public string AdafruitLightFeedName { get; set; }
    public string AdafruitTemperatureFeedName { get; set; }
    public string AdafruitMoistureFeedName { get; set; }
    public string AdafruitAnnounceFeedName { get; set; }
    public string AdafruitSensorFeedName { get; set; }
}