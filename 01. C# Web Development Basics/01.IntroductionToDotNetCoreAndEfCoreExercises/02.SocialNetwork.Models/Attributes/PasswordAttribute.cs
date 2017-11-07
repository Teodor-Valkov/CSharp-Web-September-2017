namespace _02.SocialNetwork.Models.Attributes
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class PasswordAttribute : ValidationAttribute
    {
        private const string symbols = "!@#$%^&*()_+<>?";

        public PasswordAttribute()
        {
            this.ErrorMessage = $"Password should contain at least one lower case, one upper case, one digit and one of the following symbols - {symbols}!";
        }

        public override bool IsValid(object value)
        {
            string passwordAsString = value as string;

            if (passwordAsString == null)
            {
                return true;
            }

            if (!passwordAsString.Any(symbol => char.IsLower(symbol)))
            {
                return false;
            }

            if (!passwordAsString.Any(symbol => char.IsUpper(symbol)))
            {
                return false;
            }

            if (!passwordAsString.Any(symbol => char.IsDigit(symbol)))
            {
                return false;
            }

            if (!passwordAsString.Any(symbol => symbols.Contains(symbol)))
            {
                return false;
            }

            return true;
        }
    }
}