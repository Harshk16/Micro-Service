using MediatR;
using Product.Service.Infrastructure.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;

namespace Product.Service.Features.ProductItems.CreateProduct;

public record CreateProductCommand(
    string Sku,
    string Name,
    decimal PriceAmount,
    string? Description) : IRequest<CreateProductResponse>;

public record CreateProductResponse(Guid Id, string Sku, string Name, decimal PriceAmount);

public class CreateProductHandler : IRequestHandler<CreateProductCommand, CreateProductResponse>
{
    private readonly ProductDbContext _context;

    public CreateProductHandler(ProductDbContext context)
    {
        _context = context;
    }

    public async Task<CreateProductResponse> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        // same normalization as ProductItem.Create
        var normalizedSku = request.Sku?.Trim().ToUpperInvariant() ?? string.Empty;

        var exists = await _context.Products
            .AnyAsync(x => x.Sku == normalizedSku, cancellationToken);

        if (exists)
            throw new InvalidOperationException($"Product with SKU '{normalizedSku}' already exists.");

        var product = ProductItem.Create(
            Guid.NewGuid(),
            request.Sku!,
            request.Name,
            request.PriceAmount,
            request.Description ?? string.Empty);

        _context.Products.Add(product);
        await _context.SaveChangesAsync(cancellationToken);

        return new CreateProductResponse(product.Id, product.Sku, product.Name, product.PriceAmount);
    }
}