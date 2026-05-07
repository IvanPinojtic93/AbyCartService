using FluentValidation;
using CartService.Application.Features.Cart.Commands.SetCart;

namespace CartService.Application.Features.Cart.Commands.SetCart;

public sealed class SetCartCommandValidator : AbstractValidator<SetCartCommand>
{
    public SetCartCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId is required.");

        RuleFor(x => x.Items)
            .NotNull().WithMessage("Items are required.")
            .NotEmpty().WithMessage("Cart must contain at least one item.");

        RuleForEach(x => x.Items).ChildRules(item =>
        {
            item.RuleFor(i => i.ProductId)
                .NotEmpty().WithMessage("Each item must have a valid ProductId.");

            item.RuleFor(i => i.ProductName)
                .NotEmpty().WithMessage("Each item must have a product name.");

            item.RuleFor(i => i.Quantity)
                .GreaterThan(0).WithMessage("Item quantity must be greater than 0.");

            item.RuleFor(i => i.Price)
                .GreaterThanOrEqualTo(0).WithMessage("Item price must not be negative.");
        });
    }
}
