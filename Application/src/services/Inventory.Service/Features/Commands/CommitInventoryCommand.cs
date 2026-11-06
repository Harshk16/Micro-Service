using Inventory.Service.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Service.Features.Commands;

public record CommitInventoryCommand(Guid ProductId, int Quantity) : IRequest<CommitInventoryResponse>;

public record CommitInventoryResponse(Guid ProductId, string Sku, int QuantityAvailable, int QuantityReserved);

public class CommitInventoryHandler : IRequestHandler<CommitInventoryCommand, CommitInventoryResponse>
{
    private readonly InventoryDbContext _context;

    public CommitInventoryHandler(InventoryDbContext context)
    {
        _context = context;
    }

    public async Task<CommitInventoryResponse> Handle(CommitInventoryCommand request, CancellationToken cancellationToken)
    {
        var inventoryItem = await _context.InventoryItems
            .FirstOrDefaultAsync(x => x.ProductId == request.ProductId, cancellationToken);

        if (inventoryItem is null)
            throw new KeyNotFoundException($"Inventory for product {request.ProductId} not found.");

        // Reserved stock is now sold
        inventoryItem.Commit(request.Quantity);
        await _context.SaveChangesAsync(cancellationToken);

        return new CommitInventoryResponse(
            inventoryItem.ProductId,
            inventoryItem.Sku,
            inventoryItem.QuantityAvailable,
            inventoryItem.QuantityReserved);
    }
}