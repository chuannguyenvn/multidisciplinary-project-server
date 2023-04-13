using Antlr4.Runtime;
using multidisciplinary_project_server;
using Server.WateringRules;

namespace Server.Services;

public class WateringService : BackgroundService
{
    private const float WATERING_RULES_EVALUATION_TIMER = 60f;

    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly AdafruitMqttService _adafruitMqttService;
    private readonly HelperService _helperService;

    public WateringService(IServiceScopeFactory serviceScopeFactory, AdafruitMqttService adafruitMqttService, HelperService helperService)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _adafruitMqttService = adafruitMqttService;
        _helperService = helperService;
    }

    protected async override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var timer = new PeriodicTimer(TimeSpan.FromSeconds(WATERING_RULES_EVALUATION_TIMER));
        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<DbContext>();

                foreach (var plantInformation in dbContext.PlantInformations.ToList())
                {
                    if (!dbContext.PlantDataLogs.Any(log => log.Id == plantInformation.Id)) continue;

                    var latestPlantDataLog = dbContext.PlantDataLogs.Where(log => log.Id == plantInformation.Id).OrderBy(log => log.Id).Last();
                    var metricValues = new MetricValues(latestPlantDataLog.LightValue, latestPlantDataLog.TemperatureValue, latestPlantDataLog.MoistureValue);

                    if (plantInformation.WateringRule == "") continue;

                    var wateringRule = _helperService.ParserWateringRuleString(plantInformation.WateringRule);
                    if (wateringRule.Evaluate(metricValues))
                        _adafruitMqttService.PublishMessage(_helperService.AnnounceTopicPath, _helperService.ConstructWaterPlantRequestMessage(plantInformation.Id));
                }
            }
        }
    }
}