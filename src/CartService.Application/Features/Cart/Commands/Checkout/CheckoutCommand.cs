using CartService.Application.Common;
using CartService.Application.Common.CQRS;
using CartService.Application.Common.Interfaces;
using CartService.Application.DTOs;
using CartService.Domain.Models;

namespace CartService.Application.Features.Cart.Commands.Checkout;

public record CheckoutCommand(
    string UserId,
    string Username,
    string Email,
    CartItem[] Items,
    string DeliveryAddress);

public sealed class CheckoutCommandHandler(
    IEventPublisher eventPublisher) : ICommandHandler<CheckoutCommand, Result<CheckoutResponse>>
{
    public Task<Result<CheckoutResponse>> HandleAsync(CheckoutCommand command, CancellationToken cancellationToken = default)
    {
        var orderId = Guid.NewGuid().ToString();
        var total = command.Items.Sum(i => i.Price * i.Quantity);

        eventPublisher.TryPublish("checkout.completed", new
        {
            orderId,
            userId = command.UserId,
            username = command.Username,
            email = command.Email,
            total,
            deliveryAddress = command.DeliveryAddress,
            items = command.Items
        });

        return Task.FromResult(Result<CheckoutResponse>.Ok(new CheckoutResponse(orderId)));
    }
}
