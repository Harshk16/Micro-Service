using MediatR;
using Microsoft.EntityFrameworkCore;
using Product.Service.Infrastructure.Persistence.Configurations;
// keep your existing using for ProductDbContext

namespace Product.Service.Features.Commands;

public record ActivateProductCommand(Guid ProductId) : IRequest<ActivateProductResponse>;

public record ActivateProductResponse(Guid ProductId, string Sku, bool IsActive, DateTime? UpdatedAtUtc);

public class ActivateProductHandler : IRequestHandler<ActivateProductCommand, ActivateProductResponse>
{
    private readonly ProductDbContext _context;

    public ActivateProductHandler(ProductDbContext context)
    {
        _context = context;
    }

    public async Task<ActivateProductResponse> Handle(ActivateProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _context.Products
            .FirstOrDefaultAsync(x => x.Id == request.ProductId, cancellationToken);

        if (product is null)
            throw new KeyNotFoundException($"Product with ID {request.ProductId} not found.");

        product.Activate();   // throws if already active
        await _context.SaveChangesAsync(cancellationToken);

        return new ActivateProductResponse(product.Id, product.Sku, product.IsActive, product.UpdatedAtUtc);
    }
}