using Communications.Responses;
using Server.Models;


namespace Server.Services;

public interface IPlantManagementService
{
    public Task<(bool success, object result)> AddPlant(int ownerId, string name, string photo);
    public Task<(bool success, string result)> RemovePlant(int plantId);
    public (bool success, string result) EditPlant(int plantId, string newName = "", string newPhoto = "", string newWateringRule = "");
    public (bool success, object result) GetPlantByUser(int userId);
    public Task<(bool success, object result)> WaterPlant(int plantId);
}

public class PlantManagementService : IPlantManagementService
{
    private readonly DbContext _dbContext;
    private readonly HelperService _helperService;
    private readonly AdafruitMqttService _adafruitMqttService;

    public PlantManagementService(DbContext dbContext, HelperService helperService, AdafruitMqttService adafruitMqttService)
    {
        _dbContext = dbContext;
        _helperService = helperService;
        _adafruitMqttService = adafruitMqttService;
    }

    public async Task<(bool success, object result)> AddPlant(int ownerId, string name, string photo)
    {
        var newPlantId = 1;
        if (_dbContext.PlantInformations.Any())
            newPlantId = _dbContext.PlantInformations.OrderBy(info => info.Id).Last().Id + 1;

        _adafruitMqttService.PublishMessage(_helperService.AnnounceTopicPath, _helperService.ConstructAddNewPlantRequestMessage(newPlantId));
        var success = await _adafruitMqttService.TryWaitForAnnounceMessage(_helperService.ConstructAddNewPlantResponseMessage(newPlantId));
        var message = success ? "Plant added." : "Adafruit did not response to the addition request.";
        if (!success) return (false, message);

        // TODO: Add recognizer service.
        var plantInformation = new PlantInformation()
        {
            Name = name,
            CreatedDate = DateTime.Today,
            Photo = photo,
            RecognizerCode = "Test",
            WateringRule = "",
            Owner = _dbContext.Users.First(u => u.Id == ownerId),
        };

        _dbContext.PlantInformations.Add(plantInformation);
        _dbContext.SaveChanges();

        return (true, message);
    }

    public async Task<(bool success, string result)> RemovePlant(int plantId)
    {
        if (!_dbContext.PlantInformations.Any(p => p.Id == plantId)) return (false, "Plant not found.");

        _adafruitMqttService.PublishMessage(_helperService.AnnounceTopicPath, _helperService.ConstructRemovePlantRequestMessage(plantId));
        var success = await _adafruitMqttService.TryWaitForAnnounceMessage(_helperService.ConstructRemovePlantResponseMessage(plantId));
        var message = success ? "Plant removed." : "Adafruit did not response to the removal request.";
        if (!success) return (false, message);

        var removingPlantInformation = new PlantInformation() {Id = plantId};
        _dbContext.PlantInformations.Remove(removingPlantInformation);
        _dbContext.SaveChanges();

        return (true, message);
    }

    public (bool success, string result) EditPlant(int plantId, string newName = "", string newPhoto = "", string newWateringRule = "")
    {
        if (!_dbContext.PlantInformations.Any(p => p.Id == plantId)) return (false, "Plant not found.");

        var editingPlantInformation = _dbContext.PlantInformations.First(p => p.Id == plantId);
        if (newName != "") editingPlantInformation.Name = newName;
        if (newPhoto != "") editingPlantInformation.Photo = newPhoto;
        if (newWateringRule != "") editingPlantInformation.WateringRule = newWateringRule;
        _dbContext.PlantInformations.Update(editingPlantInformation);
        _dbContext.SaveChanges();

        return (true, "");
    }

    public (bool success, object result) GetPlantByUser(int userId)
    {
        var plantGetResponse = new GetPlantResponse()
        {
            PlantInformations = _dbContext.PlantInformations.Where(p => p.Owner.Id == userId)
                .OrderBy(p => p.Id)
                .Select(info => new GetPlantResponseUnit()
                {
                    Id = info.Id,
                    Name = info.Name,
                    CreatedDate = info.CreatedDate,
                    Photo = info.Photo,
                    RecognizerCode = info.RecognizerCode,
                })
                .ToList(),
        };

        return (true, plantGetResponse);
    }

    public async Task<(bool success, object result)> WaterPlant(int plantId)
    {
        if (!_dbContext.PlantInformations.Any(p => p.Id == plantId)) return (false, "Plant not found.");

        _adafruitMqttService.PublishMessage(_helperService.AnnounceTopicPath, _helperService.ConstructWaterPlantRequestMessage(plantId));
        var success = await _adafruitMqttService.TryWaitForAnnounceMessage(_helperService.ConstructWaterPlantResponseMessage(plantId));
        if (!success) return (false, "Adafruit did not response to watering request.");

        var plantWaterLog = new PlantWaterLog()
        {
            Timestamp = DateTime.Now,
            WateredPlant = _dbContext.PlantInformations.First(info => info.Id == plantId),
            IsManual = true,
        };

        _dbContext.PlantWaterLogs.Add(plantWaterLog);
        _dbContext.SaveChanges();
        return (true, "Plant watered.");
    }
}