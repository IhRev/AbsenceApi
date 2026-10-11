using System.Reflection;
using Absence.Api.Common.Results;
using FluentValidation;
using MediatR;

namespace Absence.Api.Common.Behaviors;

internal sealed class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!validators.Any())
        {
            return await next();
        }

        var context = new ValidationContext<TRequest>(request);
        var messages = new List<string>();
        foreach (var validator in validators)
        {
            var result = await validator.ValidateAsync(context, cancellationToken);
            messages.AddRange(result.Errors.Select(error => error.ErrorMessage));
        }

        if (messages.Count == 0)
        {
            return await next();
        }

        var message = string.Join(" ", messages.Distinct());
        if (!TryAsBadRequest(message, out var response))
        {
            throw new InvalidOperationException(
                $"{typeof(TRequest).FullName} failed validation, but {typeof(TResponse).Name} has no BadRequest result.");
        }

        return response;
    }

    private static bool TryAsBadRequest(string message, out TResponse response)
    {
        var conversion = typeof(TResponse).GetMethod(
            "op_Implicit",
            BindingFlags.Public | BindingFlags.Static,
            binder: null,
            types: [typeof(BadRequest)],
            modifiers: null);

        if (conversion is null)
        {
            response = default!;
            return false;
        }

        response = (TResponse)conversion.Invoke(null, [new BadRequest(message)])!;
        return true;
    }
}
