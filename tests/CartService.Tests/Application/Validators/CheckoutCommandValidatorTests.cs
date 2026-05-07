using CartService.Application.Features.Cart.Commands.Checkout;
using CartService.Domain.Models;
using FluentValidation.TestHelper;

namespace CartService.Tests.Application.Validators;

public class CheckoutCommandValidatorTests
{
    private readonly CheckoutCommandValidator _validator = new();

    private static CartItem ValidItem() =>
        new(Guid.NewGuid(), "Widget", 9.99, 1, Guid.NewGuid());

    private static CheckoutCommand ValidCommand() =>
        new("user-1", "alice", "alice@example.com", [ValidItem()], "1 Main St");

    [Fact]
    public void Validate_ValidCommand_HasNoErrors()
    {
        var result = _validator.TestValidate(ValidCommand());
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    public void Validate_EmptyUserId_HasError(string userId)
    {
        var cmd = ValidCommand() with { UserId = userId };
        var result = _validator.TestValidate(cmd);
        result.ShouldHaveValidationErrorFor(x => x.UserId);
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    public void Validate_EmptyUsername_HasError(string username)
    {
        var cmd = ValidCommand() with { Username = username };
        var result = _validator.TestValidate(cmd);
        result.ShouldHaveValidationErrorFor(x => x.Username);
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    public void Validate_EmptyDeliveryAddress_HasError(string address)
    {
        var cmd = ValidCommand() with { DeliveryAddress = address };
        var result = _validator.TestValidate(cmd);
        result.ShouldHaveValidationErrorFor(x => x.DeliveryAddress);
    }

    [Fact]
    public void Validate_DeliveryAddressTooLong_HasError()
    {
        var cmd = ValidCommand() with { DeliveryAddress = new string('x', 301) };
        var result = _validator.TestValidate(cmd);
        result.ShouldHaveValidationErrorFor(x => x.DeliveryAddress);
    }

    [Fact]
    public void Validate_EmptyItemsArray_HasError()
    {
        var cmd = ValidCommand() with { Items = [] };
        var result = _validator.TestValidate(cmd);
        result.ShouldHaveValidationErrorFor(x => x.Items);
    }

    [Fact]
    public void Validate_ItemWithZeroQuantity_HasError()
    {
        var item = new CartItem(Guid.NewGuid(), "Widget", 9.99, 0, Guid.NewGuid());
        var cmd = ValidCommand() with { Items = [item] };
        var result = _validator.TestValidate(cmd);
        result.ShouldHaveAnyValidationError();
    }

    [Fact]
    public void Validate_ItemWithNegativePrice_HasError()
    {
        var item = new CartItem(Guid.NewGuid(), "Widget", -0.01, 1, Guid.NewGuid());
        var cmd = ValidCommand() with { Items = [item] };
        var result = _validator.TestValidate(cmd);
        result.ShouldHaveAnyValidationError();
    }
}
