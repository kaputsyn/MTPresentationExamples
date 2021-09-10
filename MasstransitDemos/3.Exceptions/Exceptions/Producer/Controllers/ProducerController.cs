using Contracts;
using MassTransit;
using Microsoft.AspNetCore.Mvc;

namespace Producer.Controllers;
[ApiController]
[Route("[controller]")]
public class ProducerController : ControllerBase
{
   
    private readonly ILogger<ProducerController> _logger;
    private readonly IBusControl _busControl;

    public ProducerController(ILogger<ProducerController> logger, IBusControl busControl)
    {
        _logger = logger;
        _busControl = busControl;
    }

    [HttpGet]
    public async Task<IActionResult> Get(int messageCount)
    {
        for (int i = 0; i < messageCount; i++) 
        {
            await _busControl.Publish<ITransienrErrorCommand>(new
            {
                ErrorCode = 1
            });
        }
        

        return Ok();
    }
}
