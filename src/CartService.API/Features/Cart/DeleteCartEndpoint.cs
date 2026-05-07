using CartService.API.Extensions;
using CartService.Application.Common;
using CartService.Application.Common.CQRS;
using CartService.Application.Features.Cart.Commands.DeleteCart;
using System.Security.Claims;

namespace CartService.API.Features.Cart;

public class DeleteCartEndpoint : IEndpoint
{
    public static void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapDelete("/cart", HandleAsync).RequireAuthorization();
    }

    private static async Task<IResult> HandleAsync(
        ClaimsPrincipal principal,
        ICommandHandler<DeleteCartCommand, Result> handler)
    {
        var userId = principal.FindFirstValue(ClaimTypes.Name);
        if (userId is null) return Results.Unauthorized();

        var result = await handler.HandleAsync(new DeleteCartCommand(userId));
        return result.IsSuccess
            ? Results.Ok()
            : Results.Problem(result.Error, statusCode: result.StatusCode);
    }
}
