namespace _04.BankSystem.Client.Factories
{
    using _04.BankSystem.Client.Utilities;
    using _04.BankSystem.Models;
    using _04.BankSystem.Models.Contracts.Factories;
    using _04.BankSystem.Models.Contracts.Validators;
    using _04.BankSystem.Models.Validators;
    using System;

    public class CheckingAccountFactory : ICheckingAccountFactory
    {
        public CheckingAccount GenerateCheckingAccount(string[] accountArgs)
        {
            string accountNumber = AccountNumberGenerator.GenerateAccountNumber();
            decimal balance = decimal.Parse(accountArgs[0]);
            decimal fee = decimal.Parse(accountArgs[1]);

            CheckingAccount checkingAccount = new CheckingAccount(accountNumber, balance, fee);

            IValidationResult validationResult = CheckingAccountValidator.IsValid(checkingAccount);

            if (!validationResult.IsValid)
            {
                throw new ArgumentException(string.Join($"{Environment.NewLine}", validationResult.ValidationErrors));
            }

            return checkingAccount;
        }
    }
}