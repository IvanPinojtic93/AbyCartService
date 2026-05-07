using CartService.Application.Features.Cart.Commands.SetCart;
using CartService.Domain.Models;
using FluentValidation.TestHelper;

namespace CartService.Tests.Application.Validators;

public class SetCartCommandValidatorTests
{
    private readonly SetCartCommandValidator _validator = new();

    private static CartItem ValidItem() =>
        new(Guid.NewGuid(), "Widget", 9.99, 1, Guid.NewGuid());

    [Fact]
    public void Validate_ValidCommand_HasNoErrors()
    {
        var result = _validator.TestValidate(new SetCartCommand("user-1", [ValidItem()]));
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    public void Validate_EmptyUserId_HasError(string userId)
    {
        var result = _validator.TestValidate(new SetCartCommand(userId, [ValidItem()]));
        result.ShouldHaveValidationErrorFor(x => x.UserId);
    }

    [Fact]
    public void Validate_EmptyItemsArray_HasError()
    {
        var result = _validator.TestValidate(new SetCartCommand("user-1", []));
        result.ShouldHaveValidationErrorFor(x => x.Items);
    }

    [Fact]
    public void Validate_ItemWithZeroQuantity_HasError()
    {
        var item = new CartItem(Guid.NewGuid(), "Widget", 9.99, 0, Guid.NewGuid());
        var result = _validator.TestValidate(new SetCartCommand("user-1", [item]));
        result.ShouldHaveAnyValidationError();
    }

    [Fact]
    public void Validate_ItemWithNegativePrice_HasError()
    {
        var item = new CartItem(Guid.NewGuid(), "Widget", -1.00, 1, Guid.NewGuid());
        var result = _validator.TestValidate(new SetCartCommand("user-1", [item]));
        result.ShouldHaveAnyValidationError();
    }

    [Fact]
    public void Validate_ItemWithEmptyProductName_HasError()
    {
        var item = new CartItem(Guid.NewGuid(), "", 9.99, 1, Guid.NewGuid());
        var result = _validator.TestValidate(new SetCartCommand("user-1", [item]));
        result.ShouldHaveAnyValidationError();
    }

    [Fact]
    public void Validate_ItemWithEmptyProductId_HasError()
    {
        var item = new CartItem(Guid.Empty, "Widget", 9.99, 1, Guid.NewGuid());
        var result = _validator.TestValidate(new SetCartCommand("user-1", [item]));
        result.ShouldHaveAnyValidationError();
    }
}
