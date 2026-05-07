using CartService.Application.Common.Interfaces;
using CartService.Application.Features.Cart.Commands.SetCart;
using CartService.Domain.Models;

namespace CartService.Tests.Application.Commands;

public class SetCartCommandHandlerTests
{
    private const string UserId = "user-1";

    private readonly ICartRepository _cartRepository;
    private readonly SetCartCommandHandler _handler;

    public SetCartCommandHandlerTests()
    {
        _cartRepository = Substitute.For<ICartRepository>();
        _handler = new SetCartCommandHandler(_cartRepository);
    }

    [Fact]
    public async Task HandleAsync_ValidCommand_ReturnsOk()
    {
        var items = new[] { new CartItem(Guid.NewGuid(), "Widget", 9.99, 2, Guid.NewGuid()) };
        var command = new SetCartCommand(UserId, items);

        var result = await _handler.HandleAsync(command);

        result.IsSuccess.Should().BeTrue();
        result.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task HandleAsync_ValidCommand_CallsRepositorySetAsync()
    {
        var items = new[] { new CartItem(Guid.NewGuid(), "Widget", 9.99, 1, Guid.NewGuid()) };
        var command = new SetCartCommand(UserId, items);

        await _handler.HandleAsync(command);

        await _cartRepository.Received(1).SetAsync(UserId, items, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_EmptyItems_ReturnsOk()
    {
        var command = new SetCartCommand(UserId, []);

        var result = await _handler.HandleAsync(command);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task HandleAsync_MultipleItems_StoresAllItems()
    {
        var items = new[]
        {
            new CartItem(Guid.NewGuid(), "Item A", 5.00, 3, Guid.NewGuid()),
            new CartItem(Guid.NewGuid(), "Item B", 15.00, 1, Guid.NewGuid())
        };
        var command = new SetCartCommand("user-2", items);

        await _handler.HandleAsync(command);

        await _cartRepository.Received(1).SetAsync("user-2", Arg.Is<CartItem[]>(a => a.Length == 2), Arg.Any<CancellationToken>());
    }
}
