using FluentValidation.TestHelper;
using HoroscopeApi.DTOs.Requests;
using HoroscopeApi.Validators;

namespace HoroscopeApi.Tests.Validators;

public class UpdateProfileRequestValidatorTests
{
    private readonly UpdateProfileRequestValidator _validator = new();

    [Fact]
    public void WhenAllFieldsEmpty_FailsValidation()
    {
        var model = new UpdateProfileRequestDto();

        var result = _validator.TestValidate(model);

        Assert.False(result.IsValid);
    }

    [Fact]
    public void WhenOnlyEmailProvided_PassesValidation()
    {
        var model = new UpdateProfileRequestDto { Email = "nuevo@test.com" };

        var result = _validator.TestValidate(model);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void WhenOnlyBirthDateProvided_PassesValidation()
    {
        var model = new UpdateProfileRequestDto { BirthDate = new DateOnly(1990, 10, 5) };

        var result = _validator.TestValidate(model);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void WhenEmailProvidedButInvalid_FailsValidation()
    {
        var model = new UpdateProfileRequestDto { Email = "no-es-email" };

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.Email);
    }
}
