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

    [HttpGet("{plantId}/latest")]
    public IActionResult GetLatestData([FromRoute] int plantId)
    {
        var (success, content) = _plantDataService.GetLatestData(plantId);
        if (!success) return BadRequest(content);
        return Ok(content);
    }

    [HttpGet("{plantId}/lasthour")]
    public IActionResult GetLastHourData([FromRoute] int plantId)
    {
        var (success, content) = _plantDataService.GetLastHourData(plantId);
        if (!success) return BadRequest(content);
        return Ok(content);
    }

    [HttpGet("{plantId}/last24hours")]
    public IActionResult GetLast24HoursData([FromRoute] int plantId)
    {
        var (success, content) = _plantDataService.GetLast24HoursData(plantId);
        if (!success) return BadRequest(content);
        return Ok(content);
    }
}