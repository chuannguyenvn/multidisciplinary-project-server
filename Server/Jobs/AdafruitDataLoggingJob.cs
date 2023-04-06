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
        var client = _httpClientFactory.CreateClient();
        var response = await client.SendAsync(_httpMessageCreationService.CreateAdafruitSensorFeedRequest());
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