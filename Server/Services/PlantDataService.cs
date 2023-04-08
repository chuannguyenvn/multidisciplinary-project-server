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
        var plantInformation = _dbContext.PlantInformations.First(p => p.Id == plantId);
        var plantIndex = _dbContext.PlantInformations.ToList().IndexOf(plantInformation);

        var latestLightLog = _dbContext.PlantLogs.Last(l => l.ValueString[0] == 'L');
        var latestTemperatureLog = _dbContext.PlantLogs.Last(l => l.ValueString[0] == 'T');
        var latestMoistureLog = _dbContext.PlantLogs.Last(l => l.ValueString[0] == 'M');

        var lightValues = latestLightLog.ValueString.Split(';');
        var temperatureValues = latestTemperatureLog.ValueString.Split(';');
        var moistureValues = latestMoistureLog.ValueString.Split(';');

        return new PlantLatestDataResponse()
        {
            LightValue = float.Parse(lightValues[plantIndex]),
            TemperatureValue = float.Parse(temperatureValues[plantIndex]),
            MoistureValue = float.Parse(moistureValues[plantIndex]),
        };
    }
}