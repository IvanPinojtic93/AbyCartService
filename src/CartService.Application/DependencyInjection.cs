using CartService.Application.Common;
using CartService.Application.Common.Behaviors;
using CartService.Application.Common.CQRS;
using CartService.Application.DTOs;
using CartService.Application.Features.Cart.Commands.Checkout;
using CartService.Application.Features.Cart.Commands.DeleteCart;
using CartService.Application.Features.Cart.Commands.SetCart;
using CartService.Application.Features.Cart.Queries.GetCart;
using CartService.Domain.Models;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace CartService.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Validators
        services.AddScoped<IValidator<SetCartCommand>, SetCartCommandValidator>();
        services.AddScoped<IValidator<CheckoutCommand>, CheckoutCommandValidator>();

        // Concrete handlers
        services.AddScoped<SetCartCommandHandler>();
        services.AddScoped<CheckoutCommandHandler>();
        services.AddScoped<DeleteCartCommandHandler>();

        // Decorated handlers (validation + inner handler)
        services.AddScoped<ICommandHandler<SetCartCommand, Result>>(sp =>
            new ValidatingCommandHandler<SetCartCommand, Result>(
                sp.GetRequiredService<SetCartCommandHandler>(),
                sp.GetServices<IValidator<SetCartCommand>>(),
                Result.Fail));

        services.AddScoped<ICommandHandler<CheckoutCommand, Result<CheckoutResponse>>>(sp =>
            new ValidatingCommandHandler<CheckoutCommand, Result<CheckoutResponse>>(
                sp.GetRequiredService<CheckoutCommandHandler>(),
                sp.GetServices<IValidator<CheckoutCommand>>(),
                Result<CheckoutResponse>.Fail));

        services.AddScoped<ICommandHandler<DeleteCartCommand, Result>>(sp =>
            sp.GetRequiredService<DeleteCartCommandHandler>());

        // Queries
        services.AddScoped<IQueryHandler<GetCartQuery, Result<CartResponse>>, GetCartQueryHandler>();

        return services;
    }
}
