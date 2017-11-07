namespace Judge.App.Infrastructure.Helpers
{
    public static class ErrorConstants
    {
        public const string EmailError = "<p>Email is required. Emails must have at least one '@' and one '.' symbols.</p>";
        public const string PasswordError = "<p>Password is required. Passwords must be at least 6 symbols and must contain at least 1 uppercase, 1 lowercase letter and 1 digit.</p>";
        public const string ConfirmPasswordErrorError = "<p>Confirm password is required. Confirm password must match the provided password.</p>";

        public const string EmailExistsError = "<p>E-mail is already taken.</p>";
        public const string LoginError = "<p>Invalid credentials.</p>";

        public static string ContestFirstLetterError = "<p>Contest name must start with capital letter.</p>";
        public static string ContestLengthError = "<p>Contest name is required. Contest name must should be between 3 and 100 symbols long.</p>";

        public static string SubmissionCodeError = "<p>Submission code cannot be empty! Please refresh and try again.</p>";
    }
}