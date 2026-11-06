using MediatR;
using Microsoft.AspNetCore.Mvc;
using Product.Service.Features.Commands;
using Product.Service.Features.ProductItems.CreateProduct;
using Product.Service.Features.ProductItems.GetProduct;

namespace Product.Service.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class ProductsController : ControllerBase
{
    private readonly ISender _mediator;

    public ProductsController(ISender mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateProductCommand command)
    {
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(Get), new { id = result.Id }, result);
    }

    [HttpGet()]
    public async Task<IActionResult> Get([FromQuery] Guid? id)
    {
        var result = await _mediator.Send(new GetProductQuery(id));
        return result is not null ? Ok(result) : NotFound();
    }

    [HttpPost()]
    public async Task<IActionResult> Update([FromBody] UpdateProductCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpPost()]
    public async Task<IActionResult> Activate([FromBody] ActivateProductCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpPost()]
    public async Task<IActionResult> Deactivate([FromBody] DeactivateProductCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }
}
