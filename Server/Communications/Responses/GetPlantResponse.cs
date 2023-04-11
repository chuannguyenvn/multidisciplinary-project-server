using Server.Models;

namespace Communications.Responses;

public class GetPlantResponse
{
    public List<PlantInformation> PlantInformations { get; set; }
}