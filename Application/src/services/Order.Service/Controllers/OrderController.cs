using MediatR;
using Microsoft.AspNetCore.Mvc;
using Order.Api.Features.Commands;
using Order.Api.Features.Queries;
using Order.Service.Controllers;
using Order.Service.Features.Commands;

namespace Order.Api.Controllers;

public class OrderController : BaseController
{
    private readonly IMediator _mediator;

    public OrderController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetOrder(Guid orderId)
    {
        try
        {
            var response = await _mediator.Send(new GetOrderQuery(orderId));
            return Ok(response);
        }
        catch (Exception ex)
        {
            return HandleException(ex);
        }
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrder(CreateOrderCommand command)
    {
        try
        {
            var response = await _mediator.Send(command);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return HandleException(ex);
        }
    }

    [HttpPost]
    public async Task<IActionResult> CancelOrder(CancelOrderCommand command)
    {
        try
        {
            var response = await _mediator.Send(command);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return HandleException(ex);
        }
    }

    // INTERNAL: the saga will call Confirm after payment is approved.
    // Remove this action when the broker flow is added.
    [HttpPost]
    public async Task<IActionResult> ConfirmOrder(ConfirmOrderCommand command)
    {
        try
        {
            var response = await _mediator.Send(command);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return HandleException(ex);
        }
    }
}