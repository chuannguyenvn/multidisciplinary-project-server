using System.Security.Claims;
using System.Security.Cryptography;
using Communications.Requests;
using Communications.Responses;
using Cronos;
using Server.Models;
using Server.WateringRules;


namespace Server.Services;

public interface IPlantManagementService
{
    public Task<(bool success, object result)> AddPlant(ClaimsPrincipal user, string name);
    public Task<(bool success, string result)> RemovePlant(ClaimsPrincipal user, int plantId);
    public (bool success, string result) EditPlant(ClaimsPrincipal user, int plantId, EditPlantRequest editPlantRequest);
    public (bool success, object result) GetPlantsOfCurrentUser(ClaimsPrincipal user);
    public Task<(bool success, object result)> WaterPlant(ClaimsPrincipal user, int plantId);
    public Task<bool> TryWaterPlant(int plantId);
}

public class PlantManagementService : IPlantManagementService
{
    private const float WAIT_FOR_MESSAGE_TIMEOUT = 10f;

    private readonly DbContext _dbContext;
    private readonly HelperService _helperService;
    private readonly AdafruitMqttService _adafruitMqttService;

    public PlantManagementService(DbContext dbContext, HelperService helperService, AdafruitMqttService adafruitMqttService)
    {
        _dbContext = dbContext;
        _helperService = helperService;
        _adafruitMqttService = adafruitMqttService;
    }

    public async Task<(bool success, object result)> AddPlant(ClaimsPrincipal user, string name)
    {
        var newPlantId = 1;
        if (_dbContext.PlantInformations.Any())
            newPlantId = _dbContext.PlantInformations.OrderBy(info => info.Id).Last().Id + 1;

        _adafruitMqttService.PublishMessage(_helperService.AnnounceTopicPath, _helperService.ConstructAddNewPlantRequestMessage(newPlantId));
        var success = await TryWaitForAnnounceMessage(_helperService.ConstructAddNewPlantResponseMessage(newPlantId));
        var message = success ? "Plant added." : "Adafruit did not response to the addition request.";
        if (!success) return (false, message);

        int userId = int.Parse(user.FindFirst("id").Value);

        // TODO: Add recognizer service.
        var plantInformation = new PlantInformation()
        {
            Name = name,
            CreatedDate = DateTime.Today,
            RecognizerCode = "Test",
            WateringRuleRepeats = "",
            WateringRuleMetrics = "",
            Owner = _dbContext.Users.First(u => u.Id == userId),
        };

        _dbContext.PlantInformations.Add(plantInformation);
        _dbContext.SaveChanges();

        return (true, message);
    }

    public async Task<(bool success, string result)> RemovePlant(ClaimsPrincipal user, int plantId)
    {
        if (!_helperService.DoesPlantIdExist(plantId)) return (false, "Plant ID does not exist.");
        if (!_helperService.DoesUserOwnThisPlant(user, plantId)) return (false, "The current user does not own this plant.");

        _adafruitMqttService.PublishMessage(_helperService.AnnounceTopicPath, _helperService.ConstructRemovePlantRequestMessage(plantId));
        var success = await TryWaitForAnnounceMessage(_helperService.ConstructRemovePlantResponseMessage(plantId));
        var message = success ? "Plant removed." : "Adafruit did not response to the removal request.";
        if (!success) return (false, message);

        var removingPlantInformation = new PlantInformation() {Id = plantId};
        _dbContext.PlantInformations.Remove(removingPlantInformation);
        _dbContext.SaveChanges();

        return (true, message);
    }

    public (bool success, string result) EditPlant(ClaimsPrincipal user, int plantId, EditPlantRequest editPlantRequest)
    {
        if (!_helperService.DoesPlantIdExist(plantId)) return (false, "Plant ID does not exist.");
        if (!_helperService.DoesUserOwnThisPlant(user, plantId)) return (false, "The current user does not own this plant.");
        
        var editingPlantInformation = _dbContext.PlantInformations.First(p => p.Id == plantId);
        if (editPlantRequest.NewName != "") editingPlantInformation.Name = editPlantRequest.NewName;
        if (editPlantRequest.NewWateringRuleRepeats != "")
        {
            (bool success, CronExpression _) = _helperService.TryParserCronString(editPlantRequest.NewWateringRuleRepeats);
            if (success) editingPlantInformation.WateringRuleRepeats = editPlantRequest.NewWateringRuleRepeats;
            else return (false, "Invalid watering rule: " + editPlantRequest.NewWateringRuleRepeats);
        }
        if (editPlantRequest.NewWateringRuleMetrics != "")
        {
            (bool success, WateringRule _) = _helperService.TryParserWateringRuleString(editPlantRequest.NewWateringRuleMetrics);
            if (success) editingPlantInformation.WateringRuleMetrics = editPlantRequest.NewWateringRuleMetrics;
            else return (false, "Invalid watering rule: " + editPlantRequest.NewWateringRuleMetrics);
        }

        _dbContext.PlantInformations.Update(editingPlantInformation);
        _dbContext.SaveChanges();

        return (true, "");
    }

    public (bool success, object result) GetPlantsOfCurrentUser(ClaimsPrincipal user)
    {
        int userId = int.Parse(user.FindFirst("id").Value);

        var plantGetResponse = new GetPlantResponse()
        {
            PlantInformations = _dbContext.PlantInformations.Where(p => p.Owner.Id == userId)
                .OrderBy(p => p.Id)
                .Select(info => new GetPlantResponseUnit()
                {
                    Id = info.Id,
                    Name = info.Name,
                    CreatedDate = info.CreatedDate,
                    RecognizerCode = info.RecognizerCode,
                    WateringRuleRepeats = info.WateringRuleRepeats,
                    WateringRuleMetrics = info.WateringRuleMetrics,
                })
                .ToList(),
        };

        return (true, plantGetResponse);
    }

    public async Task<(bool success, object result)> WaterPlant(ClaimsPrincipal user, int plantId)
    {
        if (!_helperService.DoesPlantIdExist(plantId)) return (false, "Plant ID does not exist.");
        if (!_helperService.DoesUserOwnThisPlant(user, plantId)) return (false, "The current user does not own this plant.");

        var success = await TryWaterPlant(plantId);
        return success ? (true, "Plant watered.") : (false, "Adafruit did not response to watering request.");
    }

    public async Task<bool> TryWaterPlant(int plantId)
    {
        _adafruitMqttService.PublishMessage(_helperService.AnnounceTopicPath, _helperService.ConstructWaterPlantRequestMessage(plantId));
        var success = await TryWaitForAnnounceMessage(_helperService.ConstructWaterPlantResponseMessage(plantId));
        if (!success) return false;

        var plantWaterLog = new PlantWaterLog()
        {
            Timestamp = DateTime.UtcNow,
            LoggedPlant = _dbContext.PlantInformations.First(info => info.Id == plantId),
            IsManual = true,
        };

        _dbContext.PlantWaterLogs.Add(plantWaterLog);
        await _dbContext.SaveChangesAsync();
        return true;
    }

    private async Task<bool> TryWaitForAnnounceMessage(string message)
    {
        bool correctMessageReceived = false;
        float timeLeft = WAIT_FOR_MESSAGE_TIMEOUT;
        const float step = 1f;

        void MessageArrivedHandler(string m)
        {
            correctMessageReceived = m == message;
        }

        _adafruitMqttService.AnnounceMessageArrived += MessageArrivedHandler;

        while (timeLeft > 0)
        {
            if (correctMessageReceived) break;
            await Task.Delay((int)(step * 1000));
            timeLeft -= step;
        }

        _adafruitMqttService.AnnounceMessageArrived -= MessageArrivedHandler;

        return correctMessageReceived;
    }
}