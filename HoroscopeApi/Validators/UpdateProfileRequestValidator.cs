using FluentValidation;
using HoroscopeApi.Constants;
using HoroscopeApi.DTOs.Requests;

namespace HoroscopeApi.Validators;

public class UpdateProfileRequestValidator : AbstractValidator<UpdateProfileRequestDto>
{
    public UpdateProfileRequestValidator()
    {
        RuleFor(x => x)
            .Must(HaveAtLeastOneField)
            .WithMessage(Messages.Validation.EmptyUpdate);

        When(x => !string.IsNullOrWhiteSpace(x.Email), () =>
        {
            RuleFor(x => x.Email)
                .Matches(RegexPatterns.Email).WithMessage(Messages.Validation.EmailInvalid)
                .MaximumLength(150);
        });

        When(x => x.BirthDate.HasValue, () =>
        {
            RuleFor(x => x.BirthDate!.Value)
                .LessThanOrEqualTo(_ => DateOnly.FromDateTime(DateTime.Today))
                .WithMessage(Messages.Validation.BirthDateNotFuture);
        });

        When(x => !string.IsNullOrWhiteSpace(x.Password), () =>
        {
            RuleFor(x => x.Password)
                .MinimumLength(6).WithMessage(Messages.Validation.PasswordMinLength)
                .MaximumLength(100);
        });
    }

    private static bool HaveAtLeastOneField(UpdateProfileRequestDto request)
        => !string.IsNullOrWhiteSpace(request.Email)
           || request.BirthDate.HasValue
           || !string.IsNullOrWhiteSpace(request.Password);
}
