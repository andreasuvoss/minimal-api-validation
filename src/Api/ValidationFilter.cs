using System.Net;
using System.Reflection;
using FluentValidation;

namespace Api;

[AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
public class ValidateAttribute : Attribute
{
}

public static class ValidationFilter
{
    /// <summary>
    /// Validate all arguments decorated with Validate attribute
    /// </summary>
    /// <param name="context"></param>
    /// <param name="next"></param>
    /// <returns></returns>
    public static EndpointFilterDelegate ValidationFilterFactory(EndpointFilterFactoryContext context,
        EndpointFilterDelegate next)
    {
        var validationDescriptors = GetValidators(context.MethodInfo, context.ApplicationServices).ToList();

        if (validationDescriptors.Any())
        {
            return invocationContext => Validate(validationDescriptors, invocationContext, next);
        }

        return invocationContext => next(invocationContext);
    }

    /// <summary>
    /// Where the actual validation happens
    /// </summary>
    /// <param name="validationDescriptors"></param>
    /// <param name="invocationContext"></param>
    /// <param name="next"></param>
    /// <returns></returns>
    private static async ValueTask<object?> Validate(IEnumerable<ValidationDescriptor> validationDescriptors,
        EndpointFilterInvocationContext invocationContext, EndpointFilterDelegate next)
    {
        foreach (var descriptor in validationDescriptors)
        {
            var argument = invocationContext.Arguments[descriptor.ArgumentIndex];

            if (argument is null) continue;

            var validationResult = await descriptor.Validator.ValidateAsync(
                new ValidationContext<object>(argument)
            );

            if (!validationResult.IsValid)
            {
                return Results.ValidationProblem(validationResult.ToDictionary(),
                    statusCode: (int)HttpStatusCode.UnprocessableEntity);
            }
        }

        return await next.Invoke(invocationContext);
    }

    /// <summary>
    /// Gets all parameters decorated with the Validate attribute in the given method
    /// </summary>
    /// <param name="methodInfo">The method info</param>
    /// <param name="serviceProvider">The service provider</param>
    /// <returns>A ValidationDescriptor containing information about the parameter and it's validator</returns>
    static IEnumerable<ValidationDescriptor> GetValidators(MethodInfo methodInfo, IServiceProvider serviceProvider)
    {
        var parameters = methodInfo.GetParameters();

        var name = methodInfo.Name;

        for (var i = 0; i < parameters.Length; i++)
        {
            var parameter = parameters[i];

            if (parameter.GetCustomAttribute<ValidateAttribute>() is null) continue;

            var validatorType = typeof(IValidator<>).MakeGenericType(parameter.ParameterType);

            var validator = serviceProvider.GetService(validatorType) as IValidator;

            if (validator is not null)
            {
                yield return new ValidationDescriptor
                {
                    ArgumentIndex = i, 
                    ArgumentType = parameter.ParameterType, 
                    Validator = validator
                };
            }
        }
    }

    private class ValidationDescriptor
    {
        public required int ArgumentIndex { get; init; }
        public required Type ArgumentType { get; init; }
        public required IValidator Validator { get; init; }
    }
}