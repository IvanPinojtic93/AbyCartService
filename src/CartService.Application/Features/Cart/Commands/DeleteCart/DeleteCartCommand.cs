using CartService.Application.Common;
using CartService.Application.Common.CQRS;
using CartService.Application.Common.Interfaces;

namespace CartService.Application.Features.Cart.Commands.DeleteCart;

public record DeleteCartCommand(string UserId);

public sealed class DeleteCartCommandHandler(
    ICartRepository cartRepository) : ICommandHandler<DeleteCartCommand, Result>
{
    public async Task<Result> HandleAsync(DeleteCartCommand command, CancellationToken cancellationToken = default)
    {
        await cartRepository.DeleteAsync(command.UserId, cancellationToken);
        return Result.Ok();
    }
}
