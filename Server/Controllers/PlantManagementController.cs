using Communications.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server.Services;

namespace Server.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class PlantManagementController : ControllerBase
{
    private readonly IPlantManagementService _plantManagementService;

    public PlantManagementController(IPlantManagementService plantManagementService)
    {
        _plantManagementService = plantManagementService;
    }

    [HttpPost("add")]
    public async Task<IActionResult> AddPlant(AddPlantRequest addPlantRequest)
    {
        var (success, result) = await _plantManagementService.AddPlant(User, addPlantRequest.Name);
        if (!success) return BadRequest(result);
        return Ok(result);
    }

    [HttpGet("{id}/remove")]
    public async Task<IActionResult> RemovePlant([FromRoute] int id)
    {
        var (success, result) = await _plantManagementService.RemovePlant(User, id);
        if (!success) return BadRequest(result);
        return Ok(result);
    }

    [HttpPost("{id}/edit")]
    public IActionResult EditPlant([FromRoute] int id, EditPlantRequest editPlantRequest)
    {
        var (success, result) = _plantManagementService.EditPlant(User, id, editPlantRequest);
        if (!success) return BadRequest(result);
        return Ok(result);
    }

    [HttpGet("get")]
    public IActionResult GetPlantByUser()
    {
        var (success, result) = _plantManagementService.GetPlantsOfCurrentUser(User);
        if (!success) return BadRequest(result);
        return Ok(result);
    }

    [HttpGet("{id}/water")]
    public async Task<IActionResult> WaterPlant([FromRoute] int id)
    {
        var (success, result) = await _plantManagementService.WaterPlant(User, id);
        if (!success) return BadRequest(result);
        return Ok(result);
    }
}