using CartService.Application.Common;
using CartService.Application.Common.CQRS;
using CartService.Application.Common.Interfaces;
using CartService.Domain.Models;

namespace CartService.Application.Features.Cart.Commands.SetCart;

public record SetCartCommand(string UserId, CartItem[] Items);

public sealed class SetCartCommandHandler(
    ICartRepository cartRepository) : ICommandHandler<SetCartCommand, Result>
{
    public async Task<Result> HandleAsync(SetCartCommand command, CancellationToken cancellationToken = default)
    {
        await cartRepository.SetAsync(command.UserId, command.Items, cancellationToken);
        return Result.Ok();
    }
}
