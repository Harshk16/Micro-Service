using Inventory.Service.Infrastructure.Persistence;
using MediatR;

namespace Inventory.Service.Features.Commands;

public record ReleaseInventoryCommand(Guid Id, Guid ProdctId, string Sku, int Quantity) : IRequest<ReleaseInventoryResponse>;

public record ReleaseInventoryResponse(Guid Id, string Sku, int QuantityAvailable);

public class ReleaseInventoryHandler : IRequestHandler<ReleaseInventoryCommand, ReleaseInventoryResponse>
{
    private readonly InventoryDbContext _context;

    public ReleaseInventoryHandler(InventoryDbContext context)
    {
        _context = context;
    }

    public async Task<ReleaseInventoryResponse> Handle(ReleaseInventoryCommand request, CancellationToken cancellationToken)
    {
        var inventoryItem = await _context.InventoryItems.FindAsync(new object[] { request.Id }, cancellationToken);

        if (inventoryItem == null)
            throw new KeyNotFoundException($"Inventory item with ID {request.Id} not found.");

        // Release the reserved stock
        inventoryItem.Release(request.Quantity);
        await _context.SaveChangesAsync(cancellationToken);

        return new ReleaseInventoryResponse(inventoryItem.Id, inventoryItem.Sku, inventoryItem.QuantityAvailable);
    }
}