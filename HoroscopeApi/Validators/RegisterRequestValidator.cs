using FluentValidation;
using HoroscopeApi.Constants;
using HoroscopeApi.DTOs.Requests;

namespace HoroscopeApi.Validators;

public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty().WithMessage(Messages.Validation.UsernameRequired)
            .Length(3, 50).WithMessage(Messages.Validation.UsernameLength);

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage(Messages.Validation.EmailRequired)
            .Matches(RegexPatterns.Email).WithMessage(Messages.Validation.EmailInvalid)
            .MaximumLength(150);

        RuleFor(x => x.BirthDate)
            .NotEmpty().WithMessage(Messages.Validation.BirthDateRequired)
            .LessThanOrEqualTo(_ => DateOnly.FromDateTime(DateTime.Today))
            .WithMessage(Messages.Validation.BirthDateNotFuture);

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage(Messages.Validation.PasswordRequired)
            .MinimumLength(6).WithMessage(Messages.Validation.PasswordMinLength)
            .MaximumLength(100);
    }
}
