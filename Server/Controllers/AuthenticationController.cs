using Communications.Requests;
using Communications.Responses;
using Microsoft.AspNetCore.Mvc;
using Server.Services;

namespace Server.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthenticationController : ControllerBase
{
    private readonly IAuthenticationService _authenticationService;

    public AuthenticationController(IAuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
    }

    [HttpPost("register")]
    public IActionResult Register(AuthenticationRequest authenticationRequest)
    {
        var (success, content) =
            _authenticationService.Register(authenticationRequest.Username, authenticationRequest.Password);

        if (!success) return BadRequest(content);

        return Login(authenticationRequest);
    }

    [HttpPost("login")]
    public IActionResult Login(AuthenticationRequest authenticationRequest)
    {
        var (success, token) =
            _authenticationService.Login(authenticationRequest.Username, authenticationRequest.Password);

        if (!success) return BadRequest(token);

        return Ok(new AuthenticationResponse() {Token = token});
    }
}