using FluentValidation;
using CartService.Application.Common.CQRS;

namespace CartService.Application.Common.Behaviors;

public sealed class ValidatingCommandHandler<TCommand, TResult>(
    ICommandHandler<TCommand, TResult> inner,
    IEnumerable<IValidator<TCommand>> validators,
    Func<string, int, TResult> failFactory)
    : ICommandHandler<TCommand, TResult>
{
    public async Task<TResult> HandleAsync(TCommand command, CancellationToken cancellationToken = default)
    {
        var errors = validators
            .Select(v => v.Validate(command))
            .Where(r => !r.IsValid)
            .SelectMany(r => r.Errors)
            .Select(e => e.ErrorMessage)
            .ToList();

        if (errors.Count > 0)
            return failFactory(string.Join("; ", errors), 400);

        return await inner.HandleAsync(command, cancellationToken);
    }
}
