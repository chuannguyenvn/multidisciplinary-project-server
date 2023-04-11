using Newtonsoft.Json;
using Quartz;
using Server.Models;
using Server.Services;

namespace Server.Jobs;

public class AdafruitDataLoggingJob : IJob
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly DbContext _dbContext;
    private readonly Settings _settings;
    private readonly HttpMessageCreationService _httpMessageCreationService;

    public AdafruitDataLoggingJob(IHttpClientFactory httpClientFactory, DbContext dbContext, Settings settings,
        HttpMessageCreationService httpMessageCreationService)
    {
        _httpClientFactory = httpClientFactory;
        _dbContext = dbContext;
        _settings = settings;
        _httpMessageCreationService = httpMessageCreationService;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        var feedLogs = await RetrieveFeedLog();

        try
        {
            CreatePlantDataLogs(feedLogs);
        }
        catch
        {
            Console.WriteLine("WARNING: Creating plant logs failed the first time. Retrying in " +
                              _settings.AdafruitRetryWaitTimerSeconds + " seconds...");

            await Task.Delay(_settings.AdafruitRetryWaitTimerSeconds);

            try
            {
                CreatePlantDataLogs(feedLogs);
            }
            catch
            {
                Console.WriteLine("WARNING: Creating plant logs failed two times in a row.");
            }
        }
    }

    private async Task<List<AdafruitFeedLog>> RetrieveFeedLog()
    {
        var client = _httpClientFactory.CreateClient();
        var response = await client.SendAsync(_httpMessageCreationService.CreateAdafruitSensorFeedRequest(10));
        var responseContent = await response.Content.ReadAsStringAsync();
        var feedLogs = JsonConvert.DeserializeObject<List<AdafruitFeedLog>>(responseContent);

        if (feedLogs == null) throw new Exception("Feed log not received.");

        return feedLogs;
    }

    private void CreatePlantDataLogs(List<AdafruitFeedLog> feedLogs)
    {
        AdafruitFeedLog latestLightLog;
        AdafruitFeedLog latestTemperatureLog;
        AdafruitFeedLog latestMoistureLog;

        try
        {
            latestLightLog = feedLogs.First(log => log.Value[0] == 'L');
            latestTemperatureLog = feedLogs.First(log => log.Value[0] == 'T');
            latestMoistureLog = feedLogs.First(log => log.Value[0] == 'M');
        }
        catch
        {
            throw new Exception("Some type of feed log is missing.");
        }

        var lightValues = latestLightLog.Value[2..].Split(';').Select(float.Parse).ToList();
        var temperatureValues = latestTemperatureLog.Value[2..].Split(';').Select(float.Parse).ToList();
        var moistureValues = latestMoistureLog.Value[2..].Split(';').Select(float.Parse).ToList();

        if (feedLogs.Count != _dbContext.PlantInformations.Count())
            Console.WriteLine("WARNING: Plant count and log count mismatch.");

        for (int i = 0; i < int.Min(feedLogs.Count, _dbContext.PlantInformations.Count()); i++)
        {
            var plantLog = new PlantDataLog()
            {
                Timestamp = DateTime.Now,
                LightValue = lightValues[i],
                TemperatureValue = temperatureValues[i],
                MoistureValue = moistureValues[i],
                Owner = _dbContext.PlantInformations.ToList()[i],
            };

            _dbContext.PlantDataLogs.Add(plantLog);
        }

        _dbContext.SaveChanges();
    }
}