using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Server.Services;

namespace Server.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class PlantDataController : ControllerBase
{
    private PlantDataService _plantDataService;

    public PlantDataController(PlantDataService plantDataService)
    {
        _plantDataService = plantDataService;
    }

    [HttpGet("{plantId}")]
    public IActionResult GetLatestPlantData([FromHeader] int plantId)
    {
        var response = _plantDataService.GetLatestPlantData(plantId);
        return Ok(response);
    }
}