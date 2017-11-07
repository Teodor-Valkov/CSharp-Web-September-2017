namespace GameStore.App.Infrastructure.Validation.Users
{
    using SimpleMvc.Framework.Attributes.Validation;
    using System.Linq;

    public class PasswordAttribute : PropertyValidationAttribute
    {
        public override bool IsValid(object value)
        {
            string password = value as string;

            if (password == null)
            {
                return true;
            }

            return password.Any(s => char.IsUpper(s))
                && password.Any(s => char.IsLower(s))
                && password.Any(s => char.IsDigit(s))
                && password.Length >= 6
                && password.Length <= 50;
        }
    }
}