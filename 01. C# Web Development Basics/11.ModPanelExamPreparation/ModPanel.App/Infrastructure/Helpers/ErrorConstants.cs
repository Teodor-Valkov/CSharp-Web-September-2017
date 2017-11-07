namespace ModPanel.App.Infrastructure.Helpers
{
    public static class ErrorConstants
    {
        public const string RegisterError = "<p>Check your form for errors.</p><p> E-mails must have at least one '@' and one '.' symbols.</p><p>Passwords must be at least 6 symbols and must contain at least 1 uppercase, 1 lowercase letter and 1 digit.</p><p>Confirm password must match the provided password.</p>";
        public const string EmailExistsError = "E-mail is already taken.";
        public const string LoginError = "<p>Invalid credentials.</p>";

        public const string UserIsNotApprovedError = "You must wait for your registration to be approved!";

        public const string CreateError = "<p>Check your form for errors.</p><p>Title must begin with uppercase letter and has length between 3 and 100 symbols (inclusive).</p><p>Content must be more than 10 symbols (inclusive).</p>";
        public const string EditError = "<p>Check your form for errors.</p><p>Title must begin with uppercase letter and has length between 3 and 100 symbols (inclusive).</p><p>Content must be more than 10 symbols (inclusive).</p>";
    }
}