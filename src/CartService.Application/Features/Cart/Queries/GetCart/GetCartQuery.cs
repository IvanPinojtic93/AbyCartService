using CartService.Application.Common;
using CartService.Application.Common.CQRS;
using CartService.Application.Common.Interfaces;
using CartService.Application.DTOs;
using CartService.Domain.Models;

namespace CartService.Application.Features.Cart.Queries.GetCart;

public record GetCartQuery(string UserId);

public sealed class GetCartQueryHandler(
    ICartRepository cartRepository) : IQueryHandler<GetCartQuery, Result<CartResponse>>
{
    public async Task<Result<CartResponse>> HandleAsync(GetCartQuery query, CancellationToken cancellationToken = default)
    {
        var items = await cartRepository.GetAsync(query.UserId, cancellationToken);
        return Result<CartResponse>.Ok(new CartResponse(items ?? []));
    }
}
