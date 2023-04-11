using System.Text;
using System.Text.Json;
using Server.Models;

namespace Server.Services;

public class HelperService
{
    private readonly Settings _settings;

    public HelperService(Settings settings)
    {
        _settings = settings;
    }

    public string DecodeMqttPayload(byte[] encodedData)
    {
        var base64EncodedBytes = Convert.FromBase64String(Convert.ToBase64String(encodedData));
        return Encoding.UTF8.GetString(base64EncodedBytes);
    }

    public string AnnounceTopicPath => _settings.AdafruitUsername + "/feeds/" + _settings.AdafruitFeedName + "." + _settings.AdafruitAnnounceFeedName;
    public string SensorTopicPath => _settings.AdafruitUsername + "/feeds/" + _settings.AdafruitFeedName + "." + _settings.AdafruitSensorFeedName;

    public string ConstructAddNewPlantMessage(int plantId)
    {
        return plantId + ";N";
    }

    public string ConstructWaterPlantMessage(int plantId)
    {
        return plantId + ";W";
    }
}