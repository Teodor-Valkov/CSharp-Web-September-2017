namespace _04.BankSystem.Models.Validators
{
    using _04.BankSystem.Models.Contracts.Validators;
    using System.Collections.Generic;

    public class ValidationResult : IValidationResult
    {
        public ValidationResult()
        {
            this.IsValid = true;
            this.ValidationErrors = new List<string>();
        }

        public bool IsValid { get; set; }

        public ICollection<string> ValidationErrors { get; set; }
    }
}