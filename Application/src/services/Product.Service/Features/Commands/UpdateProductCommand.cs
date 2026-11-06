using MediatR;
using Microsoft.EntityFrameworkCore;
using Product.Service.Infrastructure.Persistence.Configurations;
// keep your existing using for ProductDbContext

namespace Product.Service.Features.Commands;

public record UpdateProductCommand(Guid ProductId, string Name, string? Description, decimal PriceAmount) : IRequest<UpdateProductResponse>;

public record UpdateProductResponse(Guid ProductId, string Sku, string Name, string? Description, decimal PriceAmount, DateTime? UpdatedAtUtc);

public class UpdateProductHandler : IRequestHandler<UpdateProductCommand, UpdateProductResponse>
{
    private readonly ProductDbContext _context;

    public UpdateProductHandler(ProductDbContext context)
    {
        _context = context;
    }

    public async Task<UpdateProductResponse> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _context.Products
            .FirstOrDefaultAsync(x => x.Id == request.ProductId, cancellationToken);

        if (product is null)
            throw new KeyNotFoundException($"Product with ID {request.ProductId} not found.");

        product.UpdateDetails(request.Name, request.Description, request.PriceAmount);
        await _context.SaveChangesAsync(cancellationToken);

        return new UpdateProductResponse(
            product.Id,
            product.Sku,
            product.Name,
            product.Description,
            product.PriceAmount,
            product.UpdatedAtUtc);
    }
}