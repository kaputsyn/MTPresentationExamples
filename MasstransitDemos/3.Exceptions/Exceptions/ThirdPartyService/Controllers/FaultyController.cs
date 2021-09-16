using Microsoft.AspNetCore.Mvc;

namespace ThirdPartyService.Controllers;
[ApiController]
[Route("[controller]")]
public class FaultyController : ControllerBase
{
    private readonly ILogger<FaultyController> _logger;
    private readonly StartUpTimer _startUpTimer;
    public FaultyController(ILogger<FaultyController> logger, StartUpTimer startUpTimer)
    {
        _logger = logger;
        _startUpTimer = startUpTimer;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        //if (!_startUpTimer.IsReady)

        if(new Random().Next(5) != 1)
        {
            _logger.LogError("Failed to handle request. Service is not ready");
            return BadRequest();
        }
        else 
        {
            return Ok();
        }
    }
}
