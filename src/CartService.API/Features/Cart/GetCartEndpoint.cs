using CartService.API.Extensions;
using CartService.Application.Common;
using CartService.Application.Common.CQRS;
using CartService.Application.DTOs;
using CartService.Application.Features.Cart.Queries.GetCart;
using System.Security.Claims;

namespace CartService.API.Features.Cart;

public class GetCartEndpoint : IEndpoint
{
    public static void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("/cart", HandleAsync).RequireAuthorization();
    }

    private static async Task<IResult> HandleAsync(
        ClaimsPrincipal principal,
        IQueryHandler<GetCartQuery, Result<CartResponse>> handler)
    {
        var userId = principal.FindFirstValue(ClaimTypes.Name);
        if (userId is null) return Results.Unauthorized();

        var result = await handler.HandleAsync(new GetCartQuery(userId));
        return result.IsSuccess
            ? Results.Ok(result.Value)
            : Results.Problem(result.Error, statusCode: result.StatusCode);
    }
}
