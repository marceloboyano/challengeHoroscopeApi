namespace HoroscopeApi.Constants;

/// <summary>
/// Application messages, centralized and grouped by domain.
/// Avoids hardcoded strings scattered across the services and keeps
/// them consistent (and ready for future localization).
/// </summary>
public static class Messages
{
    public static class Auth
    {
        public const string UsernameTaken = "Username is already taken.";
        public const string Registered = "User registered successfully.";
        public const string InvalidCredentials = "Invalid username or password.";
        public const string LoginSuccess = "Login successful.";
    }

    public static class User
    {
        public const string NotFound = "User not found.";
        public const string EmailTaken = "Email is already registered.";
        public const string ProfileUpdated = "Profile updated successfully.";
    }

    public static class Horoscope
    {
        public const string Unavailable = "The horoscope could not be retrieved at this time.";
    }

    public static class Stats
    {
        public const string NoQueries = "There are no queries recorded yet.";
    }

    public static class General
    {
        public const string InternalError = "An internal error occurred. Please try again later.";
    }

    public static class Validation
    {
        public const string UsernameRequired = "Username is required.";
        public const string UsernameLength = "Username must be between 3 and 50 characters.";
        public const string EmailRequired = "Email is required.";
        public const string EmailInvalid = "Email is not valid.";
        public const string BirthDateRequired = "Birth date is required.";
        public const string BirthDateNotFuture = "Birth date cannot be in the future.";
        public const string PasswordRequired = "Password is required.";
        public const string PasswordMinLength = "Password must be at least 6 characters long.";
        public const string EmptyUpdate = "You must provide at least one field to update.";
    }
}
