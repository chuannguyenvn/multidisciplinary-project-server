using Communications.Requests;
using Microsoft.AspNetCore.Mvc;
using Server.Services;

namespace Server.Controllers;

[ApiController]
[Route("[controller]")]
public class TestingController : ControllerBase
{
    private readonly WateringRuleTestingService _wateringRuleTestingService;

    public TestingController(WateringRuleTestingService wateringRuleTestingService)
    {
        _wateringRuleTestingService = wateringRuleTestingService;
    }

    [HttpPost("changerule")]
    public IActionResult ChangeRule(ChangeTestingRuleRequest request)
    {
        var (success, content) = _wateringRuleTestingService.ChangeRule(request);
        if (!success) return BadRequest(content);
        return Ok(content);
    }

    [HttpPost("test")]
    public IActionResult TestAgainstMetrics(TestAgainstMetricsRequest request)
    {
        var (success, content) = _wateringRuleTestingService.TestAgainstMetrics(request);
        if (!success) return BadRequest(content);
        return Ok(content);
    }
}