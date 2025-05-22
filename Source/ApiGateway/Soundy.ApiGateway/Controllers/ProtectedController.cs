using Microsoft.AspNetCore.Mvc;
using Soundy.ApiGateway.Configurations;

namespace Soundy.ApiGateway.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProtectedController : ControllerBase
{
    private readonly ILogger<ProtectedController> _logger;

    public ProtectedController(ILogger<ProtectedController> logger)
    {
        _logger = logger;
    }

    [HttpGet("public")]
    public IActionResult PublicEndpoint()
    {
        return Ok(new { message = "This is a public endpoint, no authentication required." });
    }

    [HttpGet("secured")]
    [JwtAuthorize]
    public IActionResult SecuredEndpoint()
    {
        var userId = this.GetUserId();
        return Ok(new 
        { 
            message = "This is a secured endpoint. You are authenticated.", 
            userId 
        });
    }
} 