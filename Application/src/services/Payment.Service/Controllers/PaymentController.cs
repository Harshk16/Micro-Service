using Inventory.Service.Controllers;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Payment.Service.Features.Commands;
using Payment.Service.Features.Queries;

// TEMPORARY: for testing before the broker.
// Later, a MassTransit consumer will call these commands.
public class PaymentController : BaseController
{
    private readonly IMediator _mediator;

    public PaymentController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetPayment([FromQuery] Guid orderId)
    {
        var response = await _mediator.Send(new GetPaymentQuery(orderId));
        return Ok(response);
    }

    [HttpPost]
    public async Task<IActionResult> CreatePayment([FromBody] CreatePaymentCommand request)
    {
        var response = await _mediator.Send(request);
        return Ok(response);
    }

    [HttpPost]
    public async Task<IActionResult> ApprovePayment([FromBody] ApprovePaymentCommand request)
    {
        var response = await _mediator.Send(request);
        return Ok(response);
    }

    [HttpPost]
    public async Task<IActionResult> RejectPayment([FromBody] RejectPaymentCommand request)
    {
        var response = await _mediator.Send(request);
        return Ok(response);
    }
}