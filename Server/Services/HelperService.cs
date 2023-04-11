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

    public static string DecodeBase64(byte[] encodedData)
    {
        return Encoding.UTF8.GetString(Convert.FromBase64String(Encoding.UTF8.GetString(encodedData)));
    }

    public string AnnounceTopicPath => _settings.AdafruitUsername + "/feeds/" + _settings.AdafruitFeedName + "." + _settings.AdafruitAnnounceFeedName;

    public string ConstructAddNewPlantMessage(int plantId)
    {
        return plantId + ";N";
    }

    public string ConstructWaterPlantMessage(int plantId)
    {
        return plantId + ";W";
    }
}