using FluentValidation.TestHelper;
using HoroscopeApi.DTOs.Requests;
using HoroscopeApi.Validators;

namespace HoroscopeApi.Tests.Validators;

public class RegisterRequestValidatorTests
{
    private readonly RegisterRequestValidator _validator = new();

    private static RegisterRequestDto ValidModel() => new()
    {
        Username = "marcelo",
        Email = "marcelo@test.com",
        BirthDate = new DateOnly(1990, 10, 5),
        Password = "Passw0rd!"
    };

    [Theory]
    [InlineData("user@example.com")]
    [InlineData("first.last@sub.domain.co")]
    [InlineData("name+tag@gmail.com")]
    [InlineData("a_b-c@domain.io")]
    public void Email_WhenValid_PassesValidation(string email)
    {
        var model = ValidModel();
        model.Email = email;

        var result = _validator.TestValidate(model);

        result.ShouldNotHaveValidationErrorFor(x => x.Email);
    }

    [Theory]
    [InlineData("plainaddress")]      // sin @
    [InlineData("a@b")]               // sin dominio/TLD real
    [InlineData("user@domain")]       // sin TLD
    [InlineData("user@.com")]         // dominio vacio
    [InlineData("@domain.com")]       // sin parte local
    [InlineData("user name@dom.com")] // espacio en parte local
    [InlineData("user@domain.c")]     // TLD de 1 sola letra
    public void Email_WhenInvalid_FailsValidation(string email)
    {
        var model = ValidModel();
        model.Email = email;

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.Email);
    }
}
