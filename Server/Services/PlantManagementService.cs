using Microsoft.AspNetCore.Authorization;
using Server.Models;

namespace Server.Services;

public interface IPlantManagementService
{
    public (bool success, object result) AddPlant(int ownerId, string name, byte[] photo);
    public (bool success, string result) RemovePlant(int id);
    public (bool success, string result) EditPlant(int id, string? newName = null, byte[]? newPhoto = null);
    public (bool success, object result) GetPlantByUser(int userId);
}

public class PlantManagementService : IPlantManagementService
{
    private readonly Settings _settings;
    private readonly DbContext _dbContext;

    public PlantManagementService(Settings settings, DbContext dbContext)
    {
        _settings = settings;
        _dbContext = dbContext;
    }

    public (bool success, object result) AddPlant(int ownerId, string name, byte[] photo)
    {
        // TODO: Add recognizer service.
        var plantInformation = new PlantInformation()
        {
            Name = name,
            Photo = photo,
            CreatedDate = DateTime.Today,
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

    public (bool success, string result) EditPlant(int id, string? newName = null, byte[]? newPhoto = null)
    {
        if (!_dbContext.PlantInformations.Any(p => p.Id == id)) return (false, "Plant not found.");

        var editingPlantInformation = new PlantInformation() {Id = id};
        if (newName != null) editingPlantInformation.Name = newName;
        if (newPhoto != null) editingPlantInformation.Photo = newPhoto;
        _dbContext.PlantInformations.Update(editingPlantInformation);
        _dbContext.SaveChanges();

        return (true, "");
    }

    public (bool success, object result) GetPlantByUser(int userId)
    {
        var plantList = _dbContext.PlantInformations.Where(p => p.Owner.Id == userId).OrderBy(p => p.Id);
        return (true, plantList);
    }
}