using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server.Services;

namespace Server.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class PlantDataController : ControllerBase
{
    private readonly IPlantDataService _plantDataService;

    public PlantDataController(IPlantDataService plantDataService)
    {
        _plantDataService = plantDataService;
    }

    [HttpGet("{plantId}/latest")]
    public IActionResult GetLatestData([FromRoute] int plantId)
    {
        var (success, content) = _plantDataService.GetLatestData(User, plantId);
        if (!success) return BadRequest(content);
        return Ok(content);
    }

    [HttpGet("{plantId}/lasthour")]
    public IActionResult GetLastHourData([FromRoute] int plantId)
    {
        var (success, content) = _plantDataService.GetLastHourData(User, plantId);
        if (!success) return BadRequest(content);
        return Ok(content);
    }

    [HttpGet("{plantId}/last24hours")]
    public IActionResult GetLast24HoursData([FromRoute] int plantId)
    {
        var (success, content) = _plantDataService.GetLast24HoursData(User, plantId);
        if (!success) return BadRequest(content);
        return Ok(content);
    }
}