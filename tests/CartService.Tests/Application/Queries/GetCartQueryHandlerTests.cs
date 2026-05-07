using CartService.Application.Common.Interfaces;
using CartService.Application.Features.Cart.Queries.GetCart;
using CartService.Domain.Models;

namespace CartService.Tests.Application.Queries;

public class GetCartQueryHandlerTests
{
    private const string UserId = "user-1";

    private readonly ICartRepository _cartRepository;
    private readonly GetCartQueryHandler _handler;

    public GetCartQueryHandlerTests()
    {
        _cartRepository = Substitute.For<ICartRepository>();
        _handler = new GetCartQueryHandler(_cartRepository);
    }

    [Fact]
    public async Task HandleAsync_ExistingCart_ReturnsItems()
    {
        var items = new[] { new CartItem(Guid.NewGuid(), "Widget", 9.99, 2, Guid.NewGuid()) };
        _cartRepository.GetAsync(UserId, Arg.Any<CancellationToken>()).Returns(items);
        var query = new GetCartQuery(UserId);

        var result = await _handler.HandleAsync(query);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Items.Should().HaveCount(1);
        result.Value.Items[0].ProductName.Should().Be("Widget");
    }

    [Fact]
    public async Task HandleAsync_NullFromRepository_ReturnsEmptyItems()
    {
        _cartRepository.GetAsync(UserId, Arg.Any<CancellationToken>()).Returns((CartItem[]?)null);
        var query = new GetCartQuery(UserId);

        var result = await _handler.HandleAsync(query);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Items.Should().BeEmpty();
    }

    [Fact]
    public async Task HandleAsync_EmptyCart_ReturnsEmptyItems()
    {
        _cartRepository.GetAsync(UserId, Arg.Any<CancellationToken>()).Returns([]);
        var query = new GetCartQuery(UserId);

        var result = await _handler.HandleAsync(query);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Items.Should().BeEmpty();
    }

    [Fact]
    public async Task HandleAsync_CallsRepositoryWithCorrectUserId()
    {
        _cartRepository.GetAsync("user-42", Arg.Any<CancellationToken>()).Returns([]);
        var query = new GetCartQuery("user-42");

        await _handler.HandleAsync(query);

        await _cartRepository.Received(1).GetAsync("user-42", Arg.Any<CancellationToken>());
    }
}
