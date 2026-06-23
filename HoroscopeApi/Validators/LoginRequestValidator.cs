using FluentValidation;
using HoroscopeApi.Constants;
using HoroscopeApi.DTOs.Requests;

namespace HoroscopeApi.Validators;

public class LoginRequestValidator : AbstractValidator<LoginRequestDto>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty().WithMessage(Messages.Validation.UsernameRequired);

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage(Messages.Validation.PasswordRequired);
    }
}
