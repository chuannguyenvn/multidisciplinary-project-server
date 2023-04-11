using Communications.Responses;

namespace Server.Services;

public class PlantDataService
{
    private readonly DbContext _dbContext;

    public PlantDataService(DbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public PlantLatestDataResponse GetLatestPlantData(int plantId)
    {
        var plantInformation = _dbContext.PlantInformations.First(info => info.Id == plantId);
        var plantLog = _dbContext.PlantDataLogs.First(log => log.Owner == plantInformation);

        return new PlantLatestDataResponse()
        {
            LightValue = plantLog.LightValue,
            TemperatureValue = plantLog.TemperatureValue,
            MoistureValue = plantLog.MoistureValue,
        };
    }
}