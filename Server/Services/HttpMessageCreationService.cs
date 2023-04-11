using Server.Models;

namespace Server.Services;

public class HttpMessageCreationService
{
    private readonly Settings _settings;
    private string IoKey => "?x-aio-key=" + _settings.AdafruitKey;

    public HttpMessageCreationService(Settings settings)
    {
        _settings = settings;
    }

    private const string DOMAIN = "https://io.adafruit.com/api/v2/";

    public string ConstructFeedKey(string groupName, string feedName)
    {
        return groupName + "." + feedName;
    }

    public HttpRequestMessage CreateAdafruitGroupRequest(string username, string ioKey, string feedName)
    {
        return new HttpRequestMessage(HttpMethod.Get, DOMAIN + username + "/groups/" + feedName + IoKey);
    }

    public HttpRequestMessage CreateAdafruitSensorFeedRequest(int entryCount)
    {
        return new HttpRequestMessage(HttpMethod.Get,
            DOMAIN + _settings.AdafruitUsername + "/feeds/" +
            ConstructFeedKey(_settings.AdafruitFeedName, _settings.AdafruitSensorFeedName) + "/data?limit=" +
            entryCount + IoKey);
    }
}