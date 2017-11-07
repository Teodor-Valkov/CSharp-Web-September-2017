namespace _04.BankSystem.Models.Contracts.Validators
{
    using System.Collections.Generic;

    public interface IValidationResult
    {
        bool IsValid { get; set; }

        ICollection<string> ValidationErrors { get; set; }
    }
}