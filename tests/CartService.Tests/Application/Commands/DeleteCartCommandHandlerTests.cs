using CartService.Application.Common.Interfaces;
using CartService.Application.Features.Cart.Commands.DeleteCart;

namespace CartService.Tests.Application.Commands;

public class DeleteCartCommandHandlerTests
{
    private readonly ICartRepository _cartRepository;
    private readonly DeleteCartCommandHandler _handler;

    public DeleteCartCommandHandlerTests()
    {
        _cartRepository = Substitute.For<ICartRepository>();
        _handler = new DeleteCartCommandHandler(_cartRepository);
    }

    [Fact]
    public async Task HandleAsync_ValidCommand_ReturnsOk()
    {
        var command = new DeleteCartCommand("user-1");

        var result = await _handler.HandleAsync(command);

        result.IsSuccess.Should().BeTrue();
        result.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task HandleAsync_ValidCommand_CallsRepositoryDeleteAsync()
    {
        var command = new DeleteCartCommand("user-1");

        await _handler.HandleAsync(command);

        await _cartRepository.Received(1).DeleteAsync("user-1", Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_DifferentUserId_DeletesCorrectCart()
    {
        var command = new DeleteCartCommand("user-99");

        await _handler.HandleAsync(command);

        await _cartRepository.Received(1).DeleteAsync("user-99", Arg.Any<CancellationToken>());
    }
}
