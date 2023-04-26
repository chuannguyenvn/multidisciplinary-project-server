using Server.WateringRules;

namespace Server.Services;

public class WateringService : BackgroundService
{
    private const float WATERING_RULES_EVALUATION_TIMER = 30f;
    private const float WATERING_COOLDOWN = 60f;

    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly HelperService _helperService;

    public WateringService(IServiceScopeFactory serviceScopeFactory, HelperService helperService)
    {
        _serviceScopeFactory = serviceScopeFactory;
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
                var plantManagementService = scope.ServiceProvider.GetRequiredService<IPlantManagementService>();

                foreach (var plantInformation in dbContext.PlantInformations.ToList())
                {
                    if (!dbContext.PlantDataLogs.Any(log => log.LoggedPlant.Id == plantInformation.Id)) continue;
                    if (plantInformation.WateringRuleMetrics == "") continue;

                    if (dbContext.PlantWaterLogs.Any(log => log.LoggedPlant.Id == plantInformation.Id))
                    {
                        var latestWaterLog = dbContext.PlantWaterLogs.Where(log => log.LoggedPlant.Id == plantInformation.Id).OrderBy(log => log.Timestamp).Last();
                        if (latestWaterLog.Timestamp.AddSeconds(WATERING_COOLDOWN) > DateTime.UtcNow) continue;
                    }

                    var latestPlantDataLog = dbContext.PlantDataLogs.Where(log => log.LoggedPlant.Id == plantInformation.Id).OrderBy(log => log.Timestamp).Last();
                    var metricValues = new MetricValues(latestPlantDataLog.LightValue, latestPlantDataLog.TemperatureValue, latestPlantDataLog.MoistureValue);
                    (bool success, WateringRule wateringRule) = _helperService.TryParserWateringRuleString(plantInformation.WateringRuleMetrics);

                    if (success)
                    {
                        if (wateringRule.Evaluate(metricValues)) plantManagementService.TryWaterPlant(plantInformation.Id);
                    }
                    else
                    {
                        Console.WriteLine("Parsing watering rule failed: " + plantInformation.WateringRuleMetrics);
                    }
                }
            }
        }
    }
}