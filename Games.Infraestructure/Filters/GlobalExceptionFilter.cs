
using Gamess.Core.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Hosting;

namespace Games.Infrastructure.Filters
{
    public class GlobalExceptionFilter : IExceptionFilter
    {
        private readonly IHostEnvironment _env;

        public GlobalExceptionFilter(IHostEnvironment env)
        {
            _env = env;
        }

        public void OnException(ExceptionContext context)
        {
            var ex = context.Exception;

            // Map de excepción -> código HTTP y título
            var (status, title) = ex switch
            {
                // Ejemplos (ajusta a tus custom exceptions si las tienes):
                NotFoundException => (StatusCodes.Status404NotFound, "Recurso no encontrado"),
                BadRequestDomainException => (StatusCodes.Status400BadRequest, "Solicitud inválida"),
                ConflictException => (StatusCodes.Status409Conflict, "Conflicto"),
                BusinessRuleException => (StatusCodes.Status422UnprocessableEntity, "Regla de negocio incumplida"),
                _ => (StatusCodes.Status500InternalServerError, "Error interno del servidor")
            };

            var problem = new ProblemDetails
            {
                Status = status,
                Title = title,
                Detail = ex.Message,
                Instance = context.HttpContext.Request.Path
            };

            // Para máxima compatibilidad (sin usar ProblemDetails.Extensions)
            // armamos el payload final y solo en Development agregamos datos extra
            object payload;

            if (_env.IsDevelopment())
            {
                payload = new
                {
                    problem.Type,
                    problem.Title,
                    problem.Status,
                    problem.Detail,
                    problem.Instance,
                    exception = ex.GetType().Name,
                    stackTrace = ex.StackTrace
                };
            }
            else
            {
                payload = new
                {
                    problem.Type,
                    problem.Title,
                    problem.Status,
                    problem.Detail,
                    problem.Instance
                };
            }

            context.Result = new ObjectResult(payload) { StatusCode = status };
            context.ExceptionHandled = true;
        }
    }
}
