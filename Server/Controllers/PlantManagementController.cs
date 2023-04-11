using System.Security.Claims;
using Communications.Requests;
using Communications.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
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
    public IActionResult AddPlant(PlantAdditionRequest plantAdditionRequest)
    {
        var (success, result) = _plantManagementService.AddPlant(int.Parse(User.FindFirst("id").Value), plantAdditionRequest.Name, plantAdditionRequest.Photo);
        if (!success) return BadRequest(result);
        return Ok(result);
    }

    [HttpPost("{id}/remove")]
    public IActionResult RemovePlant([FromRoute] int id)
    {
        var (success, result) = _plantManagementService.RemovePlant(id);
        if (!success) return BadRequest(result);
        return Ok(result);
    }

    [HttpPost("{id}/edit")]
    public IActionResult EditPlant([FromRoute] int id, PlantEditRequest plantEditRequest)
    {
        var (success, result) = _plantManagementService.EditPlant(id, plantEditRequest.NewName, plantEditRequest.NewPhoto);
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
}