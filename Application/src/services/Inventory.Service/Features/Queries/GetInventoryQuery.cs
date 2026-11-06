using Inventory.Service.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Service.Features.Queries;

public record GetInventoryQuery(Guid ProductId) : IRequest<GetInventoryResponse>;

public record GetInventoryResponse(
    Guid ProductId,
    string Sku,
    int QuantityAvailable,
    int QuantityReserved);

public class GetInventoryHandler : IRequestHandler<GetInventoryQuery, GetInventoryResponse>
{
    private readonly InventoryDbContext _context;

    public GetInventoryHandler(InventoryDbContext context)
    {
        _context = context;
    }

    public async Task<GetInventoryResponse> Handle(GetInventoryQuery request, CancellationToken cancellationToken)
    {
        var inventoryItem = await _context.InventoryItems
            .AsNoTracking()   // read-only, no change tracking needed
            .FirstOrDefaultAsync(x => x.ProductId == request.ProductId, cancellationToken);

        if (inventoryItem is null)
            throw new KeyNotFoundException($"Inventory for product {request.ProductId} not found.");

        return new GetInventoryResponse(
            inventoryItem.ProductId,
            inventoryItem.Sku,
            inventoryItem.QuantityAvailable,
            inventoryItem.QuantityReserved);
    }
}