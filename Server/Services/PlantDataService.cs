using Communications.Responses;

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
        if (!DoesPlantHaveAnyLog(plantId)) return (false, "Plant does not have any log.");

        var plantLog = _dbContext.PlantDataLogs.OrderByDescending(log => log.Timestamp).Last(log => log.Owner.Id == plantId);

        return (true, new PlantDataResponse()
        {
            PlantDataRange = PlantDataRange.Latest,
            PlantDataPoints = new List<PlantDataPoint>()
            {
                new PlantDataPoint()
                {
                    Timestamp = plantLog.Timestamp,
                    LightValue = plantLog.LightValue,
                    TemperatureValue = plantLog.TemperatureValue,
                    MoistureValue = plantLog.MoistureValue,
                },
            },
        });
    }

    public (bool success, object content) GetLastHourData(int plantId)
    {
        if (!DoesPlantIdExist(plantId)) return (false, "Plant ID does not exist.");
        if (!DoesPlantHaveAnyLog(plantId)) return (false, "Plant does not have any log.");

        var plantLogs = _dbContext.PlantDataLogs.Where(log => log.Owner.Id == plantId && log.Timestamp > DateTime.Now.AddHours(-1)).OrderByDescending(log => log.Timestamp);

        var plantDataPoints = new List<PlantDataPoint>();
        foreach (var plantLog in plantLogs)
        {
            plantDataPoints.Add(new PlantDataPoint()
            {
                Timestamp = plantLog.Timestamp,
                LightValue = plantLog.LightValue,
                TemperatureValue = plantLog.TemperatureValue,
                MoistureValue = plantLog.MoistureValue,
            });
        }

        return (true, new PlantDataResponse() {PlantDataRange = PlantDataRange.LastHour, PlantDataPoints = plantDataPoints});
    }

    public (bool success, object content) GetLast24HoursData(int plantId)
    {
        if (!DoesPlantIdExist(plantId)) return (false, "Plant ID does not exist.");
        if (!DoesPlantHaveAnyLog(plantId)) return (false, "Plant does not have any log.");

        var plantLogs = _dbContext.PlantDataLogs.Where(log => log.Owner.Id == plantId && log.Timestamp > DateTime.Now.AddHours(-24)).OrderByDescending(log => log.Timestamp);

        var plantDataPoints = new List<PlantDataPoint>();
        foreach (var plantLog in plantLogs)
        {
            plantDataPoints.Add(new PlantDataPoint()
            {
                Timestamp = plantLog.Timestamp,
                LightValue = plantLog.LightValue,
                TemperatureValue = plantLog.TemperatureValue,
                MoistureValue = plantLog.MoistureValue,
            });
        }

        return (true, new PlantDataResponse() {PlantDataRange = PlantDataRange.Last24Hours, PlantDataPoints = plantDataPoints});
    }

    private bool DoesPlantIdExist(int plantId)
    {
        return _dbContext.PlantInformations.Any(info => info.Id == plantId);
    }

    private bool DoesPlantHaveAnyLog(int plantId)
    {
        return _dbContext.PlantDataLogs.Any(log => log.Owner.Id == plantId);
    }
}