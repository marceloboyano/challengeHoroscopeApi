namespace HoroscopeApi.Constants;

/// <summary>
/// Reusable regex patterns for validations (same anchored-regex technique
/// used in the DemoApi reference project).
/// </summary>
public static class RegexPatterns
{
    /// <summary>
    /// Email pragmatico estandar de la industria: parte local + "@" + dominio + TLD de 2+ letras.
    /// Acepta plus-addressing (user+tag@dom.com) y rechaza valores como "a@b" o "user@domain"
    /// que el EmailAddress() por defecto de FluentValidation aceptaria.
    /// </summary>
    public const string Email = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
}
