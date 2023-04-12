using System.Text;
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
    public string LightTopicPath => _settings.AdafruitUsername + "/feeds/" + _settings.AdafruitFeedName + "." + _settings.AdafruitLightFeedName;
    public string TemperatureTopicPath => _settings.AdafruitUsername + "/feeds/" + _settings.AdafruitFeedName + "." + _settings.AdafruitTemperatureFeedName;
    public string MoistureTopicPath => _settings.AdafruitUsername + "/feeds/" + _settings.AdafruitFeedName + "." + _settings.AdafruitMoistureFeedName;

    public string ConstructAddNewPlantRequestMessage(int plantId)
    {
        return plantId + ";N";
    }

    public string ConstructAddNewPlantResponseMessage(int plantId)
    {
        return plantId + ";ND";
    }

    public string ConstructRemovePlantRequestMessage(int plantId)
    {
        return plantId + ";R";
    }

    public string ConstructRemovePlantResponseMessage(int plantId)
    {
        return plantId + ";RD";
    }

    public string ConstructWaterPlantRequestMessage(int plantId)
    {
        return plantId + ";W";
    }

    public string ConstructWaterPlantResponseMessage(int plantId)
    {
        return plantId + ";WD";
    }
}