using CartService.API.Extensions;
using CartService.Application.Common;
using CartService.Application.Common.CQRS;
using CartService.Application.Features.Cart.Commands.SetCart;
using CartService.Domain.Models;
using System.Security.Claims;

namespace CartService.API.Features.Cart;

public record CartItemsRequest(CartItem[] Items);

public class UpdateCartEndpoint : IEndpoint
{
    public static void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPut("/cart", HandleAsync).RequireAuthorization();
    }

    private static async Task<IResult> HandleAsync(
        CartItemsRequest req,
        ClaimsPrincipal principal,
        ICommandHandler<SetCartCommand, Result> handler)
    {
        var userId = principal.FindFirstValue(ClaimTypes.Name);
        if (userId is null) return Results.Unauthorized();

        var result = await handler.HandleAsync(new SetCartCommand(userId, req.Items));
        return result.IsSuccess
            ? Results.Ok()
            : Results.Problem(result.Error, statusCode: result.StatusCode);
    }
}
