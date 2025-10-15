// Games.Infrastructure/Validators/GetByIdRequestValidator.cs
using FluentValidation;
using Games.Core.CustomEntities;

namespace Games.Infrastructure.Validators
{
    public class GetByIdRequestValidator : AbstractValidator<GetByIdRequest>
    {
        public GetByIdRequestValidator()
        {
            RuleFor(x => x.Id)
                .NotNull().WithMessage("El ID es requerido.")
                .GreaterThan(0).WithMessage("El ID debe ser mayor a 0.")
                .LessThanOrEqualTo(1_000_000).WithMessage("El ID no puede ser mayor a 1,000,000.")
                .Must(BeAValidIdFormat).WithMessage("El ID debe ser un número válido.");
        }

        private bool BeAValidIdFormat(int id)
            => id.ToString().Length <= 7; // Máximo 7 dígitos
    }
}
