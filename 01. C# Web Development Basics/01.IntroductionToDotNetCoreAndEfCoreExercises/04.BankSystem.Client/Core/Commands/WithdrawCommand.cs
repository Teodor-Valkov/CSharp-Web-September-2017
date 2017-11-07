namespace _04.BankSystem.Client.Core.Commands
{
    using _04.BankSystem.Data;
    using _04.BankSystem.Models;
    using System;
    using System.Linq;

    public class WithdrawCommand : Command
    {
        public WithdrawCommand(BankSystemDbContext database, string[] commandArgs)
            : base(database, commandArgs)
        {
        }

        public override string Execute()
        {
            if (this.CommandArgs.Length != 2)
            {
                throw new ArgumentException("Input is not valid!");
            }

            if (!AuthenticationManager.IsAuthenticated())
            {
                throw new InvalidOperationException("You should log in first!");
            }

            string accountNumber = this.CommandArgs[0];
            decimal amount = decimal.Parse(this.CommandArgs[1]);

            if (amount <= 0)
            {
                throw new ArgumentException("Withdraw amount should be positive!");
            }

            User user = AuthenticationManager.GetCurrentUser();
            SavingsAccount savingAccount = user.SavingAccounts.FirstOrDefault(a => a.AccountNumber == accountNumber);
            CheckingAccount checkingAccount = user.CheckingAccounts.FirstOrDefault(a => a.AccountNumber == accountNumber);

            if (savingAccount == null && checkingAccount == null)
            {
                throw new ArgumentException($"Account {accountNumber} does not exist!");
            }

            decimal currentBalance;

            if (savingAccount != null)
            {
                savingAccount.Balance -= amount;
                currentBalance = savingAccount.Balance;
            }
            else
            {
                checkingAccount.Balance -= amount;
                currentBalance = checkingAccount.Balance;
            }

            this.Database.SaveChanges();

            return $"Account {accountNumber} - ${currentBalance:F2}";
        }
    }
}