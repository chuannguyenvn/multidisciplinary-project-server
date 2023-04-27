using System.Security.Claims;
using Communications.Responses;
using Server.Models;

namespace Server.Services;

public interface IPlantDataService
{
    public (bool success, object content) GetLatestData(ClaimsPrincipal user, int plantId);
    public (bool success, object content) GetLastHourData(ClaimsPrincipal user, int plantId);
    public (bool success, object content) GetLast24HoursData(ClaimsPrincipal user, int plantId);
}

public class PlantDataService : IPlantDataService
{
    private readonly DbContext _dbContext;
    private readonly HelperService _helperService;

    public PlantDataService(DbContext dbContext, HelperService helperService)
    {
        _dbContext = dbContext;
        _helperService = helperService;
    }

    public (bool success, object content) GetLatestData(ClaimsPrincipal user, int plantId)
    {
        if (!_helperService.DoesPlantIdExist(plantId)) return (false, "Plant ID does not exist.");
        if (!_helperService.DoesUserOwnThisPlant(user, plantId)) return (false, "The current user does not own this plant.");
        if (!_helperService.DoesPlantHaveAnyDataLog(plantId)) return (false, "Plant does not have any log.");

        var latestPlantLog = _dbContext.PlantDataLogs.OrderBy(log => log.Timestamp).Last(log => log.LoggedPlant.Id == plantId);
        var plantWaterLogList = new List<PlantWaterPoint>();

        if (_dbContext.PlantWaterLogs.Any(log => log.LoggedPlant.Id == plantId))
        {
            var latestWaterLog = _dbContext.PlantWaterLogs.OrderBy(log => log.Timestamp).Last(log => log.LoggedPlant.Id == plantId);
            plantWaterLogList.Add(new PlantWaterPoint()
            {
                Timestamp = latestWaterLog.Timestamp,
                IsManual = latestWaterLog.IsManual,
            });
        }

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
            PlantWaterPoints = plantWaterLogList,
        });
    }

    public (bool success, object content) GetLastHourData(ClaimsPrincipal user, int plantId)
    {
        if (!_helperService.DoesPlantIdExist(plantId)) return (false, "Plant ID does not exist.");
        if (!_helperService.DoesUserOwnThisPlant(user, plantId)) return (false, "The current user does not own this plant.");
        if (!_helperService.DoesPlantHaveAnyDataLog(plantId)) return (false, "Plant does not have any log.");

        var dataLogs = _dbContext.PlantDataLogs.Where(log => log.LoggedPlant.Id == plantId && log.Timestamp > DateTime.UtcNow.AddHours(-1)).OrderByDescending(log => log.Timestamp);
        var waterLogs = _dbContext.PlantWaterLogs.Where(log => log.LoggedPlant.Id == plantId && log.Timestamp > DateTime.UtcNow.AddHours(-1)).OrderByDescending(log => log.Timestamp);

        var rawStartTimestamp = DateTime.UtcNow.AddHours(-1);
        var startTimestamp = new DateTime(rawStartTimestamp.Year, rawStartTimestamp.Month, rawStartTimestamp.Day, rawStartTimestamp.Hour, rawStartTimestamp.Minute, 0, rawStartTimestamp.Kind);
        List<PlantDataPoint> plantDataPoints = new();
        for (DateTime minute = startTimestamp; minute < DateTime.UtcNow; minute = minute.AddMinutes(1))
        {
            var start = minute;
            var end = minute.AddMinutes(1) > DateTime.UtcNow ? DateTime.UtcNow : minute.AddMinutes(1);
            var dataInTimespan = dataLogs.Where(log => log.Timestamp > start && log.Timestamp < end);
            if (!dataInTimespan.Any()) continue;
            float averageLightValue = dataInTimespan.Average(log => log.LightValue);
            float averageTemperatureValue = dataInTimespan.Average(log => log.TemperatureValue);
            float averageMoistureValue = dataInTimespan.Average(log => log.MoistureValue);
            plantDataPoints.Add(new PlantDataPoint()
            {
                Timestamp = start,
                LightValue = averageLightValue,
                TemperatureValue = averageTemperatureValue,
                MoistureValue = averageMoistureValue,
            });
        }

        var plantWaterPoints = waterLogs.Select(log => new PlantWaterPoint()
            {
                Timestamp = log.Timestamp, IsManual = log.IsManual,
            })
            .ToList();

        return (true, new PlantDataResponse() {PlantDataRange = PlantDataRange.LastHour, PlantDataPoints = plantDataPoints, PlantWaterPoints = plantWaterPoints});
    }

    public (bool success, object content) GetLast24HoursData(ClaimsPrincipal user, int plantId)
    {
        if (!_helperService.DoesPlantIdExist(plantId)) return (false, "Plant ID does not exist.");
        if (!_helperService.DoesUserOwnThisPlant(user, plantId)) return (false, "The current user does not own this plant.");
        if (!_helperService.DoesPlantHaveAnyDataLog(plantId)) return (false, "Plant does not have any log.");

        var dataLogs = _dbContext.PlantDataLogs.Where(log => log.LoggedPlant.Id == plantId && log.Timestamp > DateTime.UtcNow.AddHours(-24)).OrderByDescending(log => log.Timestamp);
        var waterLogs = _dbContext.PlantWaterLogs.Where(log => log.LoggedPlant.Id == plantId && log.Timestamp > DateTime.UtcNow.AddHours(-24)).OrderByDescending(log => log.Timestamp);

        var rawStartTimestamp = DateTime.UtcNow.AddHours(-24);
        var startTimestamp = new DateTime(rawStartTimestamp.Year, rawStartTimestamp.Month, rawStartTimestamp.Day, rawStartTimestamp.Hour, 0, 0, rawStartTimestamp.Kind);
        List<PlantDataPoint> plantDataPoints = new();
        for (DateTime hour = startTimestamp; hour < DateTime.UtcNow; hour = hour.AddHours(1))
        {
            var start = hour;
            var end = hour.AddHours(1) > DateTime.UtcNow ? DateTime.UtcNow : hour.AddHours(1);
            var dataInTimespan = dataLogs.Where(log => log.Timestamp > start && log.Timestamp < end);
            if (!dataInTimespan.Any()) continue;
            float averageLightValue = dataInTimespan.Average(log => log.LightValue);
            float averageTemperatureValue = dataInTimespan.Average(log => log.TemperatureValue);
            float averageMoistureValue = dataInTimespan.Average(log => log.MoistureValue);
            plantDataPoints.Add(new PlantDataPoint()
            {
                Timestamp = start,
                LightValue = averageLightValue,
                TemperatureValue = averageTemperatureValue,
                MoistureValue = averageMoistureValue,
            });
        }

        var plantWaterPoints = waterLogs.Select(log => new PlantWaterPoint()
            {
                Timestamp = log.Timestamp, IsManual = log.IsManual,
            })
            .ToList();

        return (true, new PlantDataResponse() {PlantDataRange = PlantDataRange.Last24Hours, PlantDataPoints = plantDataPoints, PlantWaterPoints = plantWaterPoints});
    }
}