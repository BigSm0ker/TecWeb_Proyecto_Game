// Games.Infrastructure/Validators/UserDtoValidator.cs
using FluentValidation;

using Gamess.Infraestructure.DTOs;

namespace Games.Infrastructure.Validators
{
    public class UserDtoValidator : AbstractValidator<UserDto>
    {
        public UserDtoValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("El nombre es requerido.")
                .MaximumLength(50);

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("El apellido es requerido.")
                .MaximumLength(50);

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("El email es requerido.")
                .EmailAddress().WithMessage("Email no válido.")
                .MaximumLength(100);

            RuleFor(x => x.DateOfBirth)
                .LessThan(DateTime.Today).WithMessage("La fecha de nacimiento debe ser en el pasado.")
                .Must(BeAtLeast13).WithMessage("El usuario debe tener al menos 13 años.");

            RuleFor(x => x.Telephone)
                .MaximumLength(15).When(x => !string.IsNullOrWhiteSpace(x.Telephone))
                .WithMessage("Teléfono demasiado largo.");
        }

        private bool BeAtLeast13(DateTime dob)
        {
            var today = DateTime.Today;
            var age = today.Year - dob.Year;
            if (dob.Date > today.AddYears(-age)) age--;
            return age >= 13;
        }
    }
}
