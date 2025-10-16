// Games.Infrastructure/Filters/ValidationFilter.cs
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Games.Infrastructure.Validators;

namespace Games.Infrastructure.Filters
{
    public class ValidationFilter : IAsyncActionFilter
    {
        private readonly IValidationService _validationService;
        private readonly IServiceProvider _serviceProvider;

        public ValidationFilter(IValidationService validationService, IServiceProvider serviceProvider)
        {
            _validationService = validationService;
            _serviceProvider = serviceProvider;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            foreach (var arg in context.ActionArguments.Values)
            {
                if (arg is null) continue;

                
                var validatorType = typeof(IValidator<>).MakeGenericType(arg.GetType());
                var validator = _serviceProvider.GetService(validatorType);
                if (validator is null) continue;

                
                var method = typeof(IValidationService).GetMethod(nameof(IValidationService.ValidateAsync))!;
                var generic = method.MakeGenericMethod(arg.GetType());
                var task = (Task<ValidationResult>)generic.Invoke(_validationService, new[] { arg })!;
                var result = await task;

                if (!result.IsValid)
                {
                    context.Result = new BadRequestObjectResult(new { Errors = result.Errors });
                    return;
                }
            }

            await next();
        }
    }
}
