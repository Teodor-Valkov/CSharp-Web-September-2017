namespace _04.BankSystem.Client.Core.Commands
{
    using _04.BankSystem.Data;
    using _04.BankSystem.Models;
    using System;
    using System.Linq;

    public class DeductFeeCommand : Command
    {
        public DeductFeeCommand(BankSystemDbContext database, string[] commandArgs)
            : base(database, commandArgs)
        {
        }

        public override string Execute()
        {
            if (this.CommandArgs.Length != 1)
            {
                throw new ArgumentException("Input is not valid!");
            }

            if (!AuthenticationManager.IsAuthenticated())
            {
                throw new InvalidOperationException("You should log in first!");
            }

            string accountNumber = this.CommandArgs[0];

            User user = AuthenticationManager.GetCurrentUser();
            CheckingAccount checkingAccount = user.CheckingAccounts.FirstOrDefault(a => a.AccountNumber == accountNumber);

            if (checkingAccount == null)
            {
                throw new ArgumentException($"Account {accountNumber} does not exist!");
            }

            checkingAccount.Balance -= checkingAccount.Fee;

            this.Database.SaveChanges();

            return $"Account fee deducted {accountNumber} - ${checkingAccount.Balance:F2}";
        }
    }
}