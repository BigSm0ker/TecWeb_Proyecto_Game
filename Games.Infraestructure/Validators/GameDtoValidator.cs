// Games.Infrastructure/Validators/GameDtoValidator.cs
using FluentValidation;

using Gamess.Infraestructure.DTOs;

namespace Games.Infrastructure.Validators
{
    public class GameDtoValidator : AbstractValidator<GameDto>
    {
        private static readonly HashSet<string> AllowedAgeRatings = new(StringComparer.OrdinalIgnoreCase)
        { "E", "T", "M" };

        public GameDtoValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("El título es requerido.")
                .MaximumLength(120);

            RuleFor(x => x.Genre)
                .NotEmpty().WithMessage("El género es requerido.")
                .MaximumLength(50);

            RuleFor(x => x.UploaderUserId)
                .GreaterThan(0).WithMessage("UploaderUserId debe ser mayor a 0.");

            RuleFor(x => x.AgeRating)
                .Must(ar => string.IsNullOrWhiteSpace(ar) || AllowedAgeRatings.Contains(ar))
                .WithMessage("AgeRating debe ser E, T o M.");

            RuleFor(x => x.MinAge)
                .GreaterThanOrEqualTo(0).When(x => x.MinAge.HasValue)
                .WithMessage("MinAge no puede ser negativo.");

           
            When(x => !string.IsNullOrWhiteSpace(x.AgeRating) && x.MinAge.HasValue, () =>
            {
                RuleFor(x => x).Must(x =>
                {
                    if (x.AgeRating!.Equals("E", StringComparison.OrdinalIgnoreCase)) return x.MinAge!.Value <= 12;
                    if (x.AgeRating!.Equals("T", StringComparison.OrdinalIgnoreCase)) return x.MinAge!.Value is >= 13 and <= 16;
                    if (x.AgeRating!.Equals("M", StringComparison.OrdinalIgnoreCase)) return x.MinAge!.Value >= 17;
                    return true;
                }).WithMessage("MinAge no es consistente con AgeRating (E<=12, T 13-16, M>=17).");
            });

            RuleFor(x => x.ReleaseDate)
                .LessThanOrEqualTo(DateTime.Today).When(x => x.ReleaseDate.HasValue)
                .WithMessage("ReleaseDate no puede ser futura.");

            RuleFor(x => x.CoverUrl)
                .Must(BeValidUrlOrNull).WithMessage("CoverUrl debe ser una URL válida o nula.");
        }

        private bool BeValidUrlOrNull(string? url)
            => string.IsNullOrWhiteSpace(url) || Uri.IsWellFormedUriString(url, UriKind.Absolute);
    }
}
