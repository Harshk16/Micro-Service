using MediatR;
using Microsoft.EntityFrameworkCore;
using Product.Service.Infrastructure.Persistence;
using Product.Service.Infrastructure.Persistence.Configurations;

namespace Product.Service.Features.ProductItems.GetProduct;

// 1. Query & Response
public record GetProductQuery(Guid? Id) : IRequest<GetProductResponse?>;
public record GetProductResponse(Guid Id, string Sku, string Name, decimal PriceAmount, string? Description);

// 2. Handler
public class GetProductHandler : IRequestHandler<GetProductQuery, GetProductResponse?>
{
    private readonly ProductDbContext _context;

    public GetProductHandler(ProductDbContext context)
    {
        _context = context;
    }

    public async Task<GetProductResponse?> Handle(GetProductQuery request, CancellationToken cancellationToken)
    {
        // Industry best practice: use AsNoTracking() for high performance query read operations
        return await _context.Products
            .AsNoTracking()
            .Where(p =>  p.IsActive)
            .Select(p => new GetProductResponse(p.Id, p.Sku, p.Name, p.PriceAmount, p.Description))
            .FirstOrDefaultAsync(cancellationToken);
    }
}
