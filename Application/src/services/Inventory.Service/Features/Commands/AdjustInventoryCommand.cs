using Inventory.Service.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Service.Features.Commands;

public record AdjustInventoryCommand(Guid ProductId, int Quantity) : IRequest<AdjustInventoryResponse>;

public record AdjustInventoryResponse(Guid ProductId, string Sku, int QuantityAvailable);

public class AdjustInventoryHandler : IRequestHandler<AdjustInventoryCommand, AdjustInventoryResponse>
{
    private readonly InventoryDbContext _context;

    public AdjustInventoryHandler(InventoryDbContext context)
    {
        _context = context;
    }

    public async Task<AdjustInventoryResponse> Handle(AdjustInventoryCommand request, CancellationToken cancellationToken)
    {
        var inventoryItem = await _context.InventoryItems.FirstOrDefaultAsync(x => x.ProductId == request.ProductId, cancellationToken);

        if (inventoryItem is null)
            throw new KeyNotFoundException($"Inventory for product {request.ProductId} not found.");

        // Positive adds stock, negative removes it
        inventoryItem.AdjustStock(request.Quantity);
        await _context.SaveChangesAsync(cancellationToken);

        return new AdjustInventoryResponse(
            inventoryItem.ProductId,
            inventoryItem.Sku,
            inventoryItem.QuantityAvailable);
    }
}