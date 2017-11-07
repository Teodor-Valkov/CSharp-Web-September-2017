namespace _04.BankSystem.Client.Factories
{
    using _04.BankSystem.Client.Utilities;
    using _04.BankSystem.Models;
    using _04.BankSystem.Models.Contracts.Factories;
    using _04.BankSystem.Models.Contracts.Validators;
    using _04.BankSystem.Models.Validators;
    using System;

    public class SavingsAccountFactory : ISavingsAccountFactory
    {
        public SavingsAccount GenerateSavingAccount(string[] accountArgs)
        {
            string accountNumber = AccountNumberGenerator.GenerateAccountNumber();
            decimal balance = decimal.Parse(accountArgs[0]);
            decimal interestRate = decimal.Parse(accountArgs[1]);

            SavingsAccount savingsAccount = new SavingsAccount(accountNumber, balance, interestRate);

            IValidationResult validationResult = SavingAccountValidator.IsValid(savingsAccount);

            if (!validationResult.IsValid)
            {
                throw new ArgumentException(string.Join($"{Environment.NewLine}", validationResult.ValidationErrors));
            }

            return savingsAccount;
        }
    }
}