using FluentValidation;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Payment.Service.Common.Behaviors;

// This behavior intercepts every MediatR request before it reaches the handler
public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    // Inject all validators registered for this specific request type
    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(
        TRequest request, 
        RequestHandlerDelegate<TResponse> next, 
        CancellationToken cancellationToken)
    {
        if (!_validators.Any())
        {
            return await next(); // No validator found for this request, move to the handler
        }

        // 1. Create the validation context for the incoming request record
        var context = new ValidationContext<TRequest>(request);

        // 2. Execute all validators asynchronously
        var validationResults = await Task.WhenAll(
            _validators.Select(v => v.ValidateAsync(context, cancellationToken)));

        // 3. Collect any failures
        var failures = validationResults
            .SelectMany(r => r.Errors)
            .Where(f => f != null)
            .ToList();

        // 4. If there are validation failures, block execution and throw an exception
        if (failures.Count != 0)
        {
            throw new ValidationException(failures);
        }

        // 5. If everything passes, smoothly proceed to the next step (the actual Handler)
        return await next();
    }
}
