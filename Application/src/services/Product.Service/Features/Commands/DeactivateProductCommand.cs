using MediatR;
using Microsoft.EntityFrameworkCore;
using Product.Service.Infrastructure.Persistence.Configurations;
// keep your existing using for ProductDbContext

namespace Product.Service.Features.Commands;

public record DeactivateProductCommand(Guid ProductId) : IRequest<DeactivateProductResponse>;

public record DeactivateProductResponse(Guid ProductId, string Sku, bool IsActive, DateTime? UpdatedAtUtc);

public class DeactivateProductHandler : IRequestHandler<DeactivateProductCommand, DeactivateProductResponse>
{
    private readonly ProductDbContext _context;

    public DeactivateProductHandler(ProductDbContext context)
    {
        _context = context;
    }

    public async Task<DeactivateProductResponse> Handle(DeactivateProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _context.Products
            .FirstOrDefaultAsync(x => x.Id == request.ProductId, cancellationToken);

        if (product is null)
            throw new KeyNotFoundException($"Product with ID {request.ProductId} not found.");

        product.Deactivate();   // throws if already inactive
        await _context.SaveChangesAsync(cancellationToken);

        return new DeactivateProductResponse(product.Id, product.Sku, product.IsActive, product.UpdatedAtUtc);
    }
}