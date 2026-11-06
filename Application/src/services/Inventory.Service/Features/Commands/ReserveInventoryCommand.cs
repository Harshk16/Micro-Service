using Inventory.Service.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Service.Features.Commands;

public record ReserveInventoryCommand(Guid ProductId, string Sku, int Quantity) : IRequest<ReserveInventoryResponse>;

public record ReserveInventoryResponse(Guid ProductId, string Sku, int QuantityAvailable);

public class ReserveInventoryHandler : IRequestHandler<ReserveInventoryCommand, ReserveInventoryResponse>
{
    private readonly InventoryDbContext _context;

    public ReserveInventoryHandler(InventoryDbContext context)
    {
        _context = context;
    }

    public async Task<ReserveInventoryResponse> Handle(ReserveInventoryCommand request, CancellationToken cancellationToken)
    {
        var inventoryItem = await _context.InventoryItems.FirstOrDefaultAsync(x => x.ProductId == request.ProductId, cancellationToken);
        if (inventoryItem == null)
            throw new KeyNotFoundException($"Inventory item with ID {request.ProductId} not found.");

        // Reserve the stock
        inventoryItem.Reserve(request.Quantity);
        await _context.SaveChangesAsync(cancellationToken);

        return new ReserveInventoryResponse(inventoryItem.Id, inventoryItem.Sku, inventoryItem.QuantityAvailable);
    }
}