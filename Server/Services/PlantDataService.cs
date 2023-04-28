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
        // foreach (var dataLog in _dbContext.PlantDataLogs.ToList())
        // {
        //     _dbContext.PlantDataLogs.Remove(dataLog);
        // }
        //
        // foreach (var waterLog in _dbContext.PlantWaterLogs.ToList())
        // {
        //     _dbContext.PlantWaterLogs.Remove(waterLog);
        // }
        //
        // var plantInformation = _dbContext.PlantInformations.First(info => info.Id == 1);
        //
        // List<PlantDataLog> plantDataLogs = new()
        // {
        //     new PlantDataLog()
        //     {
        //         Timestamp = new DateTime(2023, 4, 27, 6, 0, 0),
        //         LightValue = 45,
        //         TemperatureValue = 25,
        //         MoistureValue = 30,
        //         LoggedPlant = plantInformation,
        //     },
        //     new PlantDataLog()
        //     {
        //         Timestamp = new DateTime(2023, 4, 27, 7, 0, 0),
        //         LightValue = 47,
        //         TemperatureValue = 27,
        //         MoistureValue = 28, LoggedPlant = plantInformation,
        //     },
        //     new PlantDataLog()
        //     {
        //         Timestamp = new DateTime(2023, 4, 27, 8, 0, 0),
        //         LightValue = 51,
        //         TemperatureValue = 28,
        //         MoistureValue = 25, LoggedPlant = plantInformation,
        //     },
        //     new PlantDataLog()
        //     {
        //         Timestamp = new DateTime(2023, 4, 27, 9, 0, 0),
        //         LightValue = 57,
        //         TemperatureValue = 30,
        //         MoistureValue = 47, LoggedPlant = plantInformation,
        //     },
        //     new PlantDataLog()
        //     {
        //         Timestamp = new DateTime(2023, 4, 27, 10, 0, 0),
        //         LightValue = 62,
        //         TemperatureValue = 31,
        //         MoistureValue = 43, LoggedPlant = plantInformation,
        //     },
        //     new PlantDataLog()
        //     {
        //         Timestamp = new DateTime(2023, 4, 27, 11, 0, 0),
        //         LightValue = 69,
        //         TemperatureValue = 32,
        //         MoistureValue = 41, LoggedPlant = plantInformation,
        //     },
        //     new PlantDataLog()
        //     {
        //         Timestamp = new DateTime(2023, 4, 27, 12, 0, 0),
        //         LightValue = 72,
        //         TemperatureValue = 32,
        //         MoistureValue = 39, LoggedPlant = plantInformation,
        //     },
        //     new PlantDataLog()
        //     {
        //         Timestamp = new DateTime(2023, 4, 27, 13, 0, 0),
        //         LightValue = 71,
        //         TemperatureValue = 32,
        //         MoistureValue = 37, LoggedPlant = plantInformation,
        //     },
        //     new PlantDataLog()
        //     {
        //         Timestamp = new DateTime(2023, 4, 27, 14, 0, 0),
        //         LightValue = 68,
        //         TemperatureValue = 31,
        //         MoistureValue = 35, LoggedPlant = plantInformation,
        //     },
        //     new PlantDataLog()
        //     {
        //         Timestamp = new DateTime(2023, 4, 27, 15, 0, 0),
        //         LightValue = 61,
        //         TemperatureValue = 30,
        //         MoistureValue = 32, LoggedPlant = plantInformation,
        //     },
        //     new PlantDataLog()
        //     {
        //         Timestamp = new DateTime(2023, 4, 27, 16, 0, 0),
        //         LightValue = 54,
        //         TemperatureValue = 29,
        //         MoistureValue = 30, LoggedPlant = plantInformation,
        //     },
        //     new PlantDataLog()
        //     {
        //         Timestamp = new DateTime(2023, 4, 27, 17, 0, 0),
        //         LightValue = 51,
        //         TemperatureValue = 28,
        //         MoistureValue = 29, LoggedPlant = plantInformation,
        //     },
        //     new PlantDataLog()
        //     {
        //         Timestamp = new DateTime(2023, 4, 27, 18, 0, 0),
        //         LightValue = 41,
        //         TemperatureValue = 26,
        //         MoistureValue = 28, LoggedPlant = plantInformation,
        //     },
        //     new PlantDataLog()
        //     {
        //         Timestamp = new DateTime(2023, 4, 27, 19, 0, 0),
        //         LightValue = 39,
        //         TemperatureValue = 25,
        //         MoistureValue = 51, LoggedPlant = plantInformation,
        //     },
        //     new PlantDataLog()
        //     {
        //         Timestamp = new DateTime(2023, 4, 27, 20, 0, 0),
        //         LightValue = 37,
        //         TemperatureValue = 25,
        //         MoistureValue = 48, LoggedPlant = plantInformation,
        //     },
        //     new PlantDataLog()
        //     {
        //         Timestamp = new DateTime(2023, 4, 27, 21, 0, 0),
        //         LightValue = 38,
        //         TemperatureValue = 24,
        //         MoistureValue = 47, LoggedPlant = plantInformation,
        //     },
        //     new PlantDataLog()
        //     {
        //         Timestamp = new DateTime(2023, 4, 27, 22, 0, 0),
        //         LightValue = 37,
        //         TemperatureValue = 25,
        //         MoistureValue = 44, LoggedPlant = plantInformation,
        //     },
        //     new PlantDataLog()
        //     {
        //         Timestamp = new DateTime(2023, 4, 27, 23, 0, 0),
        //         LightValue = 38,
        //         TemperatureValue = 24,
        //         MoistureValue = 42, LoggedPlant = plantInformation,
        //     },
        //
        //     new PlantDataLog()
        //     {
        //         Timestamp = new DateTime(2023, 4, 28, 0, 0, 0),
        //         LightValue = 22,
        //         TemperatureValue = 25,
        //         MoistureValue = 39, LoggedPlant = plantInformation,
        //     },
        //
        //     new PlantDataLog()
        //     {
        //         Timestamp = new DateTime(2023, 4, 28, 1, 0, 0),
        //         LightValue = 21,
        //         TemperatureValue = 24,
        //         MoistureValue = 37, LoggedPlant = plantInformation,
        //     },
        //
        //     new PlantDataLog()
        //     {
        //         Timestamp = new DateTime(2023, 4, 28, 2, 0, 0),
        //         LightValue = 22,
        //         TemperatureValue = 23,
        //         MoistureValue = 34, LoggedPlant = plantInformation,
        //     },
        //
        //     new PlantDataLog()
        //     {
        //         Timestamp = new DateTime(2023, 4, 28, 3, 0, 0),
        //         LightValue = 21,
        //         TemperatureValue = 23,
        //         MoistureValue = 31, LoggedPlant = plantInformation,
        //     },
        //
        //     new PlantDataLog()
        //     {
        //         Timestamp = new DateTime(2023, 4, 28, 4, 0, 0),
        //         LightValue = 27,
        //         TemperatureValue = 24,
        //         MoistureValue = 29, LoggedPlant = plantInformation,
        //     },
        //
        //     new PlantDataLog()
        //     {
        //         Timestamp = new DateTime(2023, 4, 28, 5, 0, 0),
        //         LightValue = 32,
        //         TemperatureValue = 25,
        //         MoistureValue = 27, LoggedPlant = plantInformation,
        //     },
        //
        //     new PlantDataLog()
        //     {
        //         Timestamp = new DateTime(2023, 4, 28, 6, 0, 0),
        //         LightValue = 42,
        //         TemperatureValue = 27,
        //         MoistureValue = 25, LoggedPlant = plantInformation,
        //     },
        //
        //     new PlantDataLog()
        //     {
        //         Timestamp = new DateTime(2023, 4, 28, 7, 0, 0),
        //         LightValue = 51,
        //         TemperatureValue = 28,
        //         MoistureValue = 45, LoggedPlant = plantInformation,
        //     },
        //
        //     new PlantDataLog()
        //     {
        //         Timestamp = new DateTime(2023, 4, 28, 8, 0, 0),
        //         LightValue = 57,
        //         TemperatureValue = 30,
        //         MoistureValue = 42, LoggedPlant = plantInformation,
        //     },
        //
        //     new PlantDataLog()
        //     {
        //         Timestamp = new DateTime(2023, 4, 28, 9, 0, 0),
        //         LightValue = 61,
        //         TemperatureValue = 31,
        //         MoistureValue = 39, LoggedPlant = plantInformation,
        //     },
        //
        //     new PlantDataLog()
        //     {
        //         Timestamp = new DateTime(2023, 4, 28, 10, 0, 0),
        //         LightValue = 64,
        //         TemperatureValue = 32,
        //         MoistureValue = 36, LoggedPlant = plantInformation,
        //     },
        // };
        //
        // List<PlantWaterLog> plantWaterLogs = new()
        // {
        //     new PlantWaterLog()
        //     {
        //         Timestamp = new DateTime(2023, 4, 27, 9, 0, 0),
        //         IsManual = false, LoggedPlant = plantInformation,
        //     },
        //     new PlantWaterLog()
        //     {
        //         Timestamp = new DateTime(2023, 4, 27, 19, 0, 0),
        //         IsManual = false, LoggedPlant = plantInformation,
        //     },
        //     new PlantWaterLog()
        //     {
        //         Timestamp = new DateTime(2023, 4, 28, 7, 0, 0),
        //         IsManual = false, LoggedPlant = plantInformation,
        //     },
        // };
        //
        // foreach (var dataLog in plantDataLogs)
        // {
        //     _dbContext.PlantDataLogs.Add(dataLog);
        // }
        //
        // foreach (var waterLog in plantWaterLogs)
        // {
        //     _dbContext.PlantWaterLogs.Add(waterLog);
        // }
        //
        // _dbContext.SaveChanges();

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
        for (DateTime minute = startTimestamp; minute < DateTime.UtcNow; minute = minute.AddMinutes(2))
        {
            var start = minute;
            var end = minute.AddMinutes(1) > DateTime.UtcNow ? DateTime.UtcNow : minute.AddMinutes(2);
            var dataInTimespan = dataLogs.Where(log => log.Timestamp >= start && log.Timestamp < end);
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
            var dataInTimespan = dataLogs.Where(log => log.Timestamp >= start && log.Timestamp < end);
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