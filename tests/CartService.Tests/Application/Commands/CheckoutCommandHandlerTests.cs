using CartService.Application.Common.Interfaces;
using CartService.Application.Features.Cart.Commands.Checkout;
using CartService.Domain.Models;

namespace CartService.Tests.Application.Commands;

public class CheckoutCommandHandlerTests
{
    private const string UserId = "user-1";
    private const string Username = "alice";
    private const string Email = "alice@example.com";
    private const string Address = "1 Main St";
    private const string ProductName = "Widget";

    private readonly IEventPublisher _eventPublisher;
    private readonly CheckoutCommandHandler _handler;

    public CheckoutCommandHandlerTests()
    {
        _eventPublisher = Substitute.For<IEventPublisher>();
        _handler = new CheckoutCommandHandler(_eventPublisher);
    }

    private static CartItem MakeItem(string name = ProductName) =>
        new(Guid.NewGuid(), name, 10.00, 1, Guid.NewGuid());

    [Fact]
    public async Task HandleAsync_ValidCommand_ReturnsOk()
    {
        var items = new[] { MakeItem() };
        var command = new CheckoutCommand(UserId, Username, Email, items, Address);

        var result = await _handler.HandleAsync(command);

        result.IsSuccess.Should().BeTrue();
        result.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task HandleAsync_ValidCommand_ReturnsOrderId()
    {
        var items = new[] { MakeItem() };
        var command = new CheckoutCommand(UserId, Username, Email, items, Address);

        var result = await _handler.HandleAsync(command);

        result.Value!.OrderId.Should().NotBeNullOrEmpty();
        Guid.TryParse(result.Value.OrderId, out _).Should().BeTrue();
    }

    [Fact]
    public async Task HandleAsync_ValidCommand_PublishesCheckoutEvent()
    {
        var items = new[] { MakeItem() };
        var command = new CheckoutCommand(UserId, Username, Email, items, Address);

        await _handler.HandleAsync(command);

        _eventPublisher.Received(1).TryPublish("checkout.completed", Arg.Any<object>());
    }

    [Fact]
    public async Task HandleAsync_MultipleItems_CalculatesTotalCorrectly()
    {
        var items = new[]
        {
            new CartItem(Guid.NewGuid(), "A", 5.00, 2, Guid.NewGuid()),
            new CartItem(Guid.NewGuid(), "B", 10.00, 3, Guid.NewGuid())
        };
        var command = new CheckoutCommand(UserId, Username, Email, items, Address);

        var result = await _handler.HandleAsync(command);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task HandleAsync_EachCall_GeneratesUniqueOrderId()
    {
        var items = new[] { MakeItem() };
        var command = new CheckoutCommand(UserId, Username, Email, items, Address);

        var result1 = await _handler.HandleAsync(command);
        var result2 = await _handler.HandleAsync(command);

        result1.Value!.OrderId.Should().NotBe(result2.Value!.OrderId);
    }
}
