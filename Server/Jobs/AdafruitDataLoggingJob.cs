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
    private readonly AdafruitMqttService _adafruitMqttService;

    private List<AccumulatedPlantDataLog> _accumulatedPlantDataLogs = new();
    private DateTime _accumulatorStartTime = DateTime.Now;

    public AdafruitDataLoggingJob(IHttpClientFactory httpClientFactory, DbContext dbContext, Settings settings, HttpMessageCreationService httpMessageCreationService,
        AdafruitMqttService adafruitMqttService)
    {
        _httpClientFactory = httpClientFactory;
        _dbContext = dbContext;
        _settings = settings;
        _httpMessageCreationService = httpMessageCreationService;
        _adafruitMqttService = adafruitMqttService;

        _adafruitMqttService.SensorMessageReceived += SensorMessageReceivedHandler;
    }

    private void SensorMessageReceivedHandler(string content)
    {
        var feedLog = JsonConvert.DeserializeObject<AdafruitFeedLog>(content);
        if (feedLog == null) throw new Exception("Cannot deserialize sensor message: \n" + content);
        AccumulateNewValues(feedLog);
        Console.WriteLine("Accumulating");
    }

    public async Task Execute(IJobExecutionContext context)
    {
        if (_accumulatedPlantDataLogs.Count == 0)
        {
            Console.WriteLine("WARNING: The server did not accumulate any log in the last timespan (" + _accumulatorStartTime + " to " + DateTime.Now + ").");
            return;
        }

        foreach (var accumulatedPlantDataLog in _accumulatedPlantDataLogs)
        {
            var ownerPlant = _dbContext.PlantInformations.FirstOrDefault(info => info.Id == accumulatedPlantDataLog.PlantId);
            if (ownerPlant == null) continue;

            _dbContext.PlantDataLogs.Add(new PlantDataLog()
            {
                Timestamp = DateTime.Now,
                LightValue = accumulatedPlantDataLog.AveragedLightValue,
                TemperatureValue = accumulatedPlantDataLog.AveragedTemperatureValue,
                MoistureValue = accumulatedPlantDataLog.AveragedMoistureValue,
                Owner = ownerPlant,
            });
        }

        await _dbContext.SaveChangesAsync();

        _accumulatedPlantDataLogs = new();
        _accumulatorStartTime = DateTime.Now;
    }

    private void AccumulateNewValues(AdafruitFeedLog feedLog)
    {
        var values = feedLog.Value[2..].Split(';').Select(float.Parse).ToList();

        if (values.Count > _accumulatedPlantDataLogs.Count)
        {
            for (int i = _accumulatedPlantDataLogs.Count; i < values.Count; i++)
            {
                _accumulatedPlantDataLogs.Add(new AccumulatedPlantDataLog()
                {
                    PlantId = _dbContext.PlantInformations.ToList()[i].Id,
                });
            }
        }

        for (var i = 0; i < values.Count; i++)
        {
            switch (feedLog.Value[0])
            {
                case 'L':
                    _accumulatedPlantDataLogs[i].AccumulatedLightValue += values[i];
                    _accumulatedPlantDataLogs[i].LightValueCount++;
                    break;
                case 'T':
                    _accumulatedPlantDataLogs[i].AccumulatedTemperatureValue += values[i];
                    _accumulatedPlantDataLogs[i].TemperatureValueCount++;
                    break;
                case 'M':
                    _accumulatedPlantDataLogs[i].AccumulatedMoistureValue += values[i];
                    _accumulatedPlantDataLogs[i].MoistureValueCount++;
                    break;
            }
        }
    }
}