using Newtonsoft.Json;
using Quartz;
using Server.Models;

namespace Server.Jobs;

public class AdafruitDataLoggingJob : IJob
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly DbContext _dbContext;
    private readonly Settings _settings;

    public AdafruitDataLoggingJob(IHttpClientFactory httpClientFactory, DbContext dbContext, Settings settings)
    {
        _httpClientFactory = httpClientFactory;
        _dbContext = dbContext;
        _settings = settings;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        var client = _httpClientFactory.CreateClient();
        var response = await client.SendAsync(AdafruitConnectionHelper.ConstructAdafruitSensorFeedRequest(
            _settings.AdafruitUsername,
            _settings.AdafruitKey,
            _settings.AdafruitFeedName,
            _settings.AdafruitSensorFeedName));

        var responseContent = await response.Content.ReadAsStringAsync();

        var feedLog = JsonConvert.DeserializeObject<List<AdafruitFeedLog>>(responseContent);
        var plantLog = new PlantLog()
        {
            Type = feedLog[0].Value[0], Timestamp = DateTime.Now, ValueString = feedLog[0].Value[2..],
        };

        _dbContext.PlantLogs.Add(plantLog);
        await _dbContext.SaveChangesAsync();
    }
}

public static class AdafruitConnectionHelper
{
    private const string DOMAIN = "https://io.adafruit.com/api/v2/";

    public static string ConstructFeedKey(string groupName, string feedName)
    {
        return groupName + "." + feedName;
    }

    public static string ConstructIoKey(string key)
    {
        return "?x-aio-key=" + key;
    }

    public static HttpRequestMessage ConstructAdafruitGroupRequest(string username, string ioKey, string feedName)
    {
        return new HttpRequestMessage(HttpMethod.Get,
            DOMAIN + username + "/groups/" + feedName + ConstructIoKey(ioKey));
    }

    public static HttpRequestMessage ConstructAdafruitSensorFeedRequest(string username, string ioKey, string feedName,
        string sensorFeedName)
    {
        return new HttpRequestMessage(HttpMethod.Get,
            DOMAIN + username + "/feeds/" + ConstructFeedKey(feedName, sensorFeedName) + "/data?limit=1" +
            ConstructIoKey(ioKey));
    }
}

public static class AdafruitDataParser
{
    public static (AdafruitDataType, List<float>) ParseSensorData(string data)
    {
        AdafruitDataType dataType = data[0] switch
        {
            'L' => AdafruitDataType.Light,
            'T' => AdafruitDataType.Temperature,
            'M' => AdafruitDataType.Humidity,
            _ => throw new InvalidDataException(),
        };

        return (dataType, data[2..].Split(';').Select(float.Parse).ToList());
    }
}

public enum AdafruitDataType
{
    Light,
    Temperature,
    Humidity,
}