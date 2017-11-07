namespace _04.BankSystem.Models.Validators
{
    using _04.BankSystem.Models.Contracts.Validators;
    using System.Text.RegularExpressions;

    public static class UserValidator
    {
        public static IValidationResult IsValid(User user)
        {
            IValidationResult result = new ValidationResult();

            if (user == null)
            {
                result.IsValid = false;
                result.ValidationErrors.Add("User cannot be null!");

                return result;
            }

            Regex usernameRegex = new Regex(@"^[a-zA-Z]+[a-zA-Z0-9]{2,}$");
            if (!usernameRegex.IsMatch(user.Username))
            {
                result.IsValid = false;
                result.ValidationErrors.Add("Username not valid!");
            }

            Regex passwordRegex = new Regex(@"^(?=[a-zA-Z0-9]*[A-Z])(?=[a-zA-Z0-9]*[a-z])(?=[a-zA-Z0-9]*[0-9])[a-zA-Z0-9]{6,}$");
            if (!passwordRegex.IsMatch(user.Password))
            {
                result.IsValid = false;
                result.ValidationErrors.Add("Password not valid!");
            }

            Regex emailRegex = new Regex(@"^([a-zA-Z0-9]+[-|_|\.]?)*[a-zA-Z0-9]+@([a-zA-Z0-9]+[-]?)*[a-zA-Z0-9]+\.([a-zA-Z0-9]+[-]?)*[a-zA-Z0-9]+$");
            if (!emailRegex.IsMatch(user.Email))
            {
                result.IsValid = false;
                result.ValidationErrors.Add("Email not valid!");
            }

            return result;
        }
    }
}