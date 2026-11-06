using FluentValidation;

namespace Product.Service.Features.ProductItems.CreateProduct;

public class CreateProductValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductValidator()
    {
        RuleFor(x => x.Sku).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(255);
        RuleFor(x => x.PriceAmount).GreaterThanOrEqualTo(0);
    }
}
