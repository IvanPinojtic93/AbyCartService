using CartService.API.Extensions;
using CartService.Application.Common;
using CartService.Application.Common.CQRS;
using CartService.Application.DTOs;
using CartService.Application.Features.Cart.Commands.Checkout;
using CartService.Domain.Models;
using System.Security.Claims;

namespace CartService.API.Features.Cart;

public record CheckoutRequest(CartItem[]? Items, string? DeliveryAddress);

public class CheckoutEndpoint : IEndpoint
{
    public static void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPost("/checkout", HandleAsync).RequireAuthorization();
    }

    private static async Task<IResult> HandleAsync(
        CheckoutRequest req,
        ClaimsPrincipal principal,
        ICommandHandler<CheckoutCommand, Result<CheckoutResponse>> handler)
    {
        var username = principal.FindFirstValue(ClaimTypes.Name) ?? "unknown";
        var email = principal.FindFirstValue(ClaimTypes.Email) ?? string.Empty;
        var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier) ?? username;

        var result = await handler.HandleAsync(new CheckoutCommand(
            userId,
            username,
            email,
            req.Items ?? [],
            req.DeliveryAddress ?? string.Empty));

        return result.IsSuccess
            ? Results.Ok(result.Value)
            : Results.Problem(result.Error, statusCode: result.StatusCode);
    }
}
