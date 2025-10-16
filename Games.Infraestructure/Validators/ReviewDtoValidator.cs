using FluentValidation;
using Gamess.Infraestructure.DTOs;

namespace Games.Infrastructure.Validators
{
    public class ReviewDtoValidator : AbstractValidator<ReviewDto>
    {
        public ReviewDtoValidator()
        {
           

            RuleFor(x => x.UserId)
                .GreaterThan(0).WithMessage("UserId debe ser mayor a 0.");

            RuleFor(x => x.Content)
                .NotEmpty().WithMessage("El contenido es obligatorio.")
                .MaximumLength(500).WithMessage("El contenido no puede superar los 500 caracteres.");

            RuleFor(x => x.Score)
    .InclusiveBetween((byte)1, (byte)10).WithMessage("La puntuación debe estar entre 1 y 10.");

        }
    }
}
