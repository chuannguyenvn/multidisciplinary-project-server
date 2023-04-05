using Newtonsoft.Json;
using Quartz;
using Server.Models;

namespace Server.Tasks;

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
        var response = await client.SendAsync(new HttpRequestMessage(HttpMethod.Get,
            AdafruitConnectionHelper.GetAdafruitFeedLogApi(_settings.AdafruitUsername, _settings.AdafruitKey)));

        var feedLog = JsonConvert.DeserializeObject<AdafruitFeedLog>(await response.Content.ReadAsStringAsync());

        var plantLog = new PlantLog()
        {
            LightValue = float.Parse(feedLog.feeds.First(feed => feed.name == "light").last_value),
            TemperatureValue = float.Parse(feedLog.feeds.First(feed => feed.name == "temperature").last_value),
            MoistureValue = float.Parse(feedLog.feeds.First(feed => feed.name == "moisture").last_value),
            Timestamp = DateTime.Now,
            PlantInformation = _dbContext.PlantInformations.ToList()[0],
        };

        _dbContext.PlantLogs.Add(plantLog);
        await _dbContext.SaveChangesAsync();
    }
}

public static class AdafruitConnectionHelper
{
    public static string GetAdafruitFeedLogApi(string username, string key)
    {
        return "https://io.adafruit.com/api/v2/" + username + "/groups/dadn?x-aio-key=" + key;
    }
}

public static class AdafruitDataParser
{
    public static (AdafruitDataType, List<float>) ParseData(string data)
    {
        AdafruitDataType dataType = data[0] switch
        {
            'L' => AdafruitDataType.Light,
            'T' => AdafruitDataType.Temperature,
            'H' => AdafruitDataType.Humidity,
            _ => throw new InvalidDataException(),
        };

        return (dataType, data[1..].Split(',').Select(float.Parse).ToList());
    }
}

public enum AdafruitDataType
{
    Light,
    Temperature,
    Humidity,
}