using Communications.Responses;
using Server.Models;


namespace Server.Services;

public interface IPlantManagementService
{
    public (bool success, object result) AddPlant(int ownerId, string name, string photo);
    public (bool success, string result) RemovePlant(int plantId);
    public (bool success, string result) EditPlant(int plantId, string? newName = null, string? newPhoto = null);
    public (bool success, object result) GetPlantByUser(int userId);
    public (bool success, object result) WaterPlant(int plantId);
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

    public (bool success, object result) AddPlant(int ownerId, string name, string photo)
    {
        // TODO: Add recognizer service.
        var plantInformation = new PlantInformation()
        {
            Name = name,
            CreatedDate = DateTime.Today,
            Photo = photo,
            RecognizerCode = "Test",
            Owner = _dbContext.Users.First(u => u.Id == ownerId)
        };

        _dbContext.PlantInformations.Add(plantInformation);
        _dbContext.SaveChanges();

        return (true, "");
    }

    public (bool success, string result) RemovePlant(int plantId)
    {
        if (!_dbContext.PlantInformations.Any(p => p.Id == plantId)) return (false, "Plant not found.");

        var removingPlantInformation = new PlantInformation() {Id = plantId};
        _dbContext.PlantInformations.Remove(removingPlantInformation);
        _dbContext.SaveChanges();

        return (true, "");
    }

    public (bool success, string result) EditPlant(int plantId, string newName, string newPhoto)
    {
        if (!_dbContext.PlantInformations.Any(p => p.Id == plantId)) return (false, "Plant not found.");

        var editingPlantInformation = _dbContext.PlantInformations.First(p => p.Id == plantId);
        if (newName != "") editingPlantInformation.Name = newName;
        if (newPhoto != "") editingPlantInformation.Photo = newPhoto;
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

    public (bool success, object result) WaterPlant(int plantId)
    {
        _adafruitMqttService.PublishMessage(_helperService.AnnounceTopicPath, _helperService.ConstructWaterPlantMessage(plantId));
        return (true, null);
    }
}