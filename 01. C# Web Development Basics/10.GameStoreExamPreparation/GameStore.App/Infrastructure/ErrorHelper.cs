namespace GameStore.App.Infrastructure
{
    public static class ErrorHelper
    {
        public const string RegisterError = "<p>Check your form for errors!</p><p> Emails must have at least one '@' and one '.'!</p><p>Passwords must be at least 6 symbols and must contain at least 1 uppercase, 1 lowercase letter and 1 digit!</p><p>Confirm password must match the provided password!</p>";
        public const string EmailAlreadyTakenError = "Email is already taken!";
        public const string LoginError = "<p>Invalid credentials!</p>";

        public const string GameError = "<p>Check your form for errors!</p><p>Title has to begin with uppercase letter and has length between 3 and 100 symbols (inclusive)!</p><p>Price must be a positive number with precision up to 2 digits after floating point!</p><p>Size must be a positive number with precision up to 1 digit after floating point!</p><p>Videos should only be from YouTube!</p><p>Thumbnail URL should be empty or a plain text starting with http:// or https://!</p><p>Description must be at least 20 symbols!</p>";

        public const string FilterGamesError = "Login to filter only owned games!";
    }
}