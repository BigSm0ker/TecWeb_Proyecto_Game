// Games.Infrastructure/Validators/ReviewDtoValidator.cs
using FluentValidation;

using Gamess.Infraestructure.DTOs;

namespace Games.Infrastructure.Validators
{
    public class ReviewDtoValidator : AbstractValidator<ReviewDto>
    {
        public ReviewDtoValidator()
        {
            RuleFor(x => x.GameId)
                .GreaterThan(0).WithMessage("GameId debe ser mayor a 0.");

            RuleFor(x => x.UserId)
                .GreaterThan(0).WithMessage("UserId debe ser mayor a 0.");

            RuleFor(x => x.Content)
                .NotEmpty().WithMessage("El contenido es requerido.")
                .MaximumLength(500);

            RuleFor(x => x.Score)
                .InclusiveBetween((byte)1, (byte)10)
                .WithMessage("Score debe estar entre 1 y 10.");

            // CreatedAt lo suele poner el server; si llega, que no sea futura.
            RuleFor(x => x.CreatedAt)
                .LessThanOrEqualTo(DateTime.UtcNow)
                .WithMessage("CreatedAt no puede ser futura.");
        }
    }
}
