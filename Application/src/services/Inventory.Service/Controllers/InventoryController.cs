using Inventory.Service.Controllers;
using Inventory.Service.Features.Commands;
using Inventory.Service.Features.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Inventory.Service.Controllers;


// TEMPORARY: for testing the commands before the broker.
// Reserve, Release and Commit will move to MassTransit consumers. Delete those actions then.
public class InventoryController : BaseController
{
    private readonly IMediator _mediator;

    public InventoryController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetInventory([FromQuery] Guid productId)
    {
        try
        {
            var result = await _mediator.Send(new GetInventoryQuery(productId));
            return Ok(result);
        }
        catch (Exception ex)
        {
            return HandleException(ex);
        }
    }

    [HttpPost]
    public async Task<IActionResult> CreateInventory([FromBody] CreateInventoryCommand command)
    {
        try
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return HandleException(ex);
        }
    }

    [HttpPost]
    public async Task<IActionResult> AdjustInventory([FromBody] AdjustInventoryCommand command)
    {
        try
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return HandleException(ex);
        }
    }

    [HttpPost]
    public async Task<IActionResult> ReserveInventory([FromBody] ReserveInventoryCommand command)
    {
        try
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return HandleException(ex);
        }
    }

    [HttpPost]
    public async Task<IActionResult> ReleaseInventory([FromBody] ReleaseInventoryCommand command)
    {
        try
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return HandleException(ex);
        }
    }

    [HttpPost]
    public async Task<IActionResult> CommitInventory([FromBody] CommitInventoryCommand command)
    {
        try
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return HandleException(ex);
        }
    }
}