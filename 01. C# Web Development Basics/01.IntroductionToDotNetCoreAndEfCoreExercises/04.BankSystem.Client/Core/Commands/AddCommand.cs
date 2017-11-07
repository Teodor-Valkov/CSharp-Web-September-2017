namespace _04.BankSystem.Client.Core.Commands
{
    using _04.BankSystem.Client.Factories;
    using _04.BankSystem.Data;
    using _04.BankSystem.Models;
    using _04.BankSystem.Models.Contracts.Factories;
    using System;
    using System.Linq;

    public class AddCommand : Command
    {
        private const string SavingsAccount = "SavingsAccount";
        private const string CheckingAccount = "CheckingAccount";

        public AddCommand(BankSystemDbContext database, string[] commandArgs)
            : base(database, commandArgs)
        {
            this.SavingsAccountFactory = new SavingsAccountFactory();
            this.CheckingAccountFactory = new CheckingAccountFactory();
        }

        public ISavingsAccountFactory SavingsAccountFactory { get; set; }

        public ICheckingAccountFactory CheckingAccountFactory { get; set; }

        public override string Execute()
        {
            if (this.CommandArgs.Length != 3)
            {
                throw new ArgumentException("Input is not valid!");
            }

            if (!AuthenticationManager.IsAuthenticated())
            {
                throw new InvalidOperationException("You should log in first!");
            }

            string accountType = this.CommandArgs[0];
            string[] accountArgs = this.CommandArgs.Skip(1).ToArray();

            User user = user = AuthenticationManager.GetCurrentUser();

            switch (accountType)
            {
                case CheckingAccount:
                    CheckingAccount checkingAccount = this.CheckingAccountFactory.GenerateCheckingAccount(accountArgs);
                    checkingAccount.UserId = user.Id;

                    this.Database.CheckingAccounts.Add(checkingAccount);
                    this.Database.SaveChanges();

                    return $"Account with number {checkingAccount.AccountNumber} successfully added!";

                case SavingsAccount:
                    SavingsAccount savingAccount = this.SavingsAccountFactory.GenerateSavingAccount(accountArgs);
                    savingAccount.User.Id = user.Id;

                    this.Database.SavingAccounts.Add(savingAccount);
                    this.Database.SaveChanges();

                    return $"Account with number {savingAccount.AccountNumber} successfully added!";

                default:
                    throw new ArgumentException($"Invalid account type {accountType}!");
            }
        }
    }
}