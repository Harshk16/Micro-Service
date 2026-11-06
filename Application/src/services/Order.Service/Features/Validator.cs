using FluentValidation;
using Order.Service.Features.Commands;

namespace Product.Service.Features.ProductItems.CreateProduct;

public class CreateProductValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateProductValidator()
    {
        // RuleFor(x => x.Sku).NotEmpty().MaximumLength(50);
        // RuleFor(x => x.Name).NotEmpty().MaximumLength(255);
        // RuleFor(x => x.PriceAmount).GreaterThanOrEqualTo(0);
    }
}
