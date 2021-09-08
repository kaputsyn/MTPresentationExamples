
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Saga.Contracts;

namespace Saga.Api.Controllers;
[ApiController]
[Route("[controller]")]
public class CustomerController : ControllerBase
{
    private readonly IPublishEndpoint _publishEndpoint;

    public CustomerController(IPublishEndpoint publishEndpoint)
    {
        _publishEndpoint = publishEndpoint;
    }
    [HttpDelete]
    public async Task<IActionResult> Delete(Guid? id, string customerNumber)
    {
        if (id == null)
        {
            id = Guid.NewGuid();
        }

        await _publishEndpoint.Publish<CustomerAccountClosed>(new
        {
            CustomerId = id.Value,
            CustomerNumber = customerNumber
        });

       return Ok();

    }

}
