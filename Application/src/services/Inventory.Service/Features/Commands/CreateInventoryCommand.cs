using Inventory.Service.Features.InventoryItems;
using Inventory.Service.Infrastructure.Persistence;
using MediatR;

namespace Inventory.Service.Features.Commands;

public record CreateInventoryCommand(
    Guid ProductId,
    string Sku,
    string Name,
    int initialStock) : IRequest<CreateInventoryResponse>;

public record CreateInventoryResponse(Guid Id, string Sku, int QuantityAvailable);

public class CreateInventoryHandler : IRequestHandler<CreateInventoryCommand, CreateInventoryResponse>
{
    private readonly InventoryDbContext _context;

    public CreateInventoryHandler(InventoryDbContext context)
    {
        _context = context;
    }

    public async Task<CreateInventoryResponse> Handle(CreateInventoryCommand request, CancellationToken cancellationToken)
    {
        // Instantiate the entity using the protective factory method we wrote
        var inventoryItem = InventoryItem.Create(
            Guid.NewGuid(), 
            request.ProductId,
            request.Sku,
            request.initialStock
           );

        _context.InventoryItems.Add(inventoryItem);
        await _context.SaveChangesAsync(cancellationToken);

        return new CreateInventoryResponse(inventoryItem.Id, inventoryItem.Sku, inventoryItem.QuantityAvailable);
    }
}