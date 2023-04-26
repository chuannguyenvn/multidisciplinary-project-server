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
        var (success, result) = await _plantManagementService.AddPlant(int.Parse(User.FindFirst("id").Value), addPlantRequest.Name);
        if (!success) return BadRequest(result);
        return Ok(result);
    }

    [HttpGet("{id}/remove")]
    public async Task<IActionResult> RemovePlant([FromRoute] int id)
    {
        var (success, result) = await _plantManagementService.RemovePlant(id);
        if (!success) return BadRequest(result);
        return Ok(result);
    }

    [HttpPost("{id}/edit")]
    public IActionResult EditPlant([FromRoute] int id, EditPlantRequest editPlantRequest)
    {
        var (success, result) = _plantManagementService.EditPlant(id, editPlantRequest);
        if (!success) return BadRequest(result);
        return Ok(result);
    }

    [HttpGet("get")]
    public IActionResult GetPlantByUser()
    {
        var (success, result) = _plantManagementService.GetPlantByUser(int.Parse(User.FindFirst("id").Value));
        if (!success) return BadRequest(result);
        return Ok(result);
    }

    [HttpGet("{id}/water")]
    public async Task<IActionResult> WaterPlant([FromRoute] int id)
    {
        var (success, result) = await _plantManagementService.WaterPlant(id);
        if (!success) return BadRequest(result);
        return Ok(result);
    }
}