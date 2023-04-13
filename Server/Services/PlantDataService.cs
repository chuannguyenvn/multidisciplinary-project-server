using Communications.Responses;
using Server.Models;

namespace Server.Services;

public interface IPlantDataService
{
    public (bool success, object content) GetLatestData(int plantId);
    public (bool success, object content) GetLastHourData(int plantId);
    public (bool success, object content) GetLast24HoursData(int plantId);
}

public class PlantDataService : IPlantDataService
{
    private readonly DbContext _dbContext;

    public PlantDataService(DbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public (bool success, object content) GetLatestData(int plantId)
    {
        if (!DoesPlantIdExist(plantId)) return (false, "Plant ID does not exist.");
        if (!DoesPlantHaveAnyDataLog(plantId)) return (false, "Plant does not have any log.");

        var latestPlantLog = _dbContext.PlantDataLogs.OrderBy(log => log.Timestamp).Last(log => log.Owner.Id == plantId);
        var latestWaterLog = _dbContext.PlantWaterLogs.OrderBy(log => log.Timestamp).Last(log => log.WateredPlant.Id == plantId);

        return (true, new PlantDataResponse()
        {
            PlantDataRange = PlantDataRange.Latest,
            PlantDataPoints = new List<PlantDataPoint>()
            {
                new PlantDataPoint()
                {
                    Timestamp = latestPlantLog.Timestamp,
                    LightValue = latestPlantLog.LightValue,
                    TemperatureValue = latestPlantLog.TemperatureValue,
                    MoistureValue = latestPlantLog.MoistureValue,
                },
            },
            PlantWaterPoints = new List<PlantWaterPoint>()
            {
                new PlantWaterPoint()
                {
                    Timestamp = latestWaterLog.Timestamp,
                    IsManual = latestWaterLog.IsManual,
                },
            },
        });
    }

    public (bool success, object content) GetLastHourData(int plantId)
    {
        if (!DoesPlantIdExist(plantId)) return (false, "Plant ID does not exist.");
        if (!DoesPlantHaveAnyDataLog(plantId)) return (false, "Plant does not have any log.");

        var dataLogs = _dbContext.PlantDataLogs.Where(log => log.Owner.Id == plantId && log.Timestamp > DateTime.UtcNow.AddHours(-1)).OrderByDescending(log => log.Timestamp);
        var waterLogs = _dbContext.PlantWaterLogs.Where(log => log.WateredPlant.Id == plantId && log.Timestamp > DateTime.UtcNow.AddHours(-1)).OrderByDescending(log => log.Timestamp);

        var plantDataPoints = dataLogs.Select(log => new PlantDataPoint()
            {
                Timestamp = log.Timestamp, LightValue = log.LightValue, TemperatureValue = log.TemperatureValue, MoistureValue = log.MoistureValue,
            })
            .ToList();

        var plantWaterPoints = waterLogs.Select(log => new PlantWaterPoint()
            {
                Timestamp = log.Timestamp, IsManual = log.IsManual,
            })
            .ToList();

        return (true, new PlantDataResponse() {PlantDataRange = PlantDataRange.LastHour, PlantDataPoints = plantDataPoints, PlantWaterPoints = plantWaterPoints});
    }

    public (bool success, object content) GetLast24HoursData(int plantId)
    {
        if (!DoesPlantIdExist(plantId)) return (false, "Plant ID does not exist.");
        if (!DoesPlantHaveAnyDataLog(plantId)) return (false, "Plant does not have any log.");

        var dataLogs = _dbContext.PlantDataLogs.Where(log => log.Owner.Id == plantId && log.Timestamp > DateTime.UtcNow.AddHours(-24)).OrderByDescending(log => log.Timestamp);
        var waterLogs = _dbContext.PlantWaterLogs.Where(log => log.WateredPlant.Id == plantId && log.Timestamp > DateTime.UtcNow.AddHours(-24)).OrderByDescending(log => log.Timestamp);

        var plantDataPoints = dataLogs.Select(log => new PlantDataPoint()
            {
                Timestamp = log.Timestamp, LightValue = log.LightValue, TemperatureValue = log.TemperatureValue, MoistureValue = log.MoistureValue,
            })
            .ToList();

        var plantWaterPoints = waterLogs.Select(log => new PlantWaterPoint()
            {
                Timestamp = log.Timestamp, IsManual = log.IsManual,
            })
            .ToList();

        return (true, new PlantDataResponse() {PlantDataRange = PlantDataRange.Last24Hours, PlantDataPoints = plantDataPoints, PlantWaterPoints = plantWaterPoints});
    }

    private bool DoesPlantIdExist(int plantId)
    {
        return _dbContext.PlantInformations.Any(info => info.Id == plantId);
    }

    private bool DoesPlantHaveAnyDataLog(int plantId)
    {
        return _dbContext.PlantDataLogs.Any(log => log.Owner.Id == plantId);
    }
    
    
}