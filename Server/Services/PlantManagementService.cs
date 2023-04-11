using Communications.Responses;
using Server.Models;


namespace Server.Services;

public interface IPlantManagementService
{
    public (bool success, object result) AddPlant(int ownerId, string name, string photo);
    public (bool success, string result) RemovePlant(int id);
    public (bool success, string result) EditPlant(int id, string? newName = null, string? newPhoto = null);
    public (bool success, object result) GetPlantByUser(int userId);
}

public class PlantManagementService : IPlantManagementService
{
    private readonly DbContext _dbContext;
    private readonly HelperService _helperService;

    public PlantManagementService(DbContext dbContext, HelperService helperService)
    {
        _dbContext = dbContext;
        _helperService = helperService;
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

    public (bool success, string result) RemovePlant(int id)
    {
        if (!_dbContext.PlantInformations.Any(p => p.Id == id)) return (false, "Plant not found.");

        var removingPlantInformation = new PlantInformation() {Id = id};
        _dbContext.PlantInformations.Remove(removingPlantInformation);
        _dbContext.SaveChanges();

        return (true, "");
    }

    public (bool success, string result) EditPlant(int id, string newName, string newPhoto)
    {
        if (!_dbContext.PlantInformations.Any(p => p.Id == id)) return (false, "Plant not found.");

        var editingPlantInformation = _dbContext.PlantInformations.First(p => p.Id == id);
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
            PlantInformations = _dbContext.PlantInformations.Where(p => p.Owner.Id == userId).OrderBy(p => p.Id).ToList(),
        };

        return (true, plantGetResponse);
    }
}