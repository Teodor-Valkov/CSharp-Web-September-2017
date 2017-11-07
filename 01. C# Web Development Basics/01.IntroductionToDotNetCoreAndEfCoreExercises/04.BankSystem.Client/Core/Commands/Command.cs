namespace _04.BankSystem.Client.Core.Commands
{
    using _04.BankSystem.Data;
    using _04.BankSystem.Models.Contracts.Core;

    public abstract class Command : ICommand
    {
        public Command(BankSystemDbContext database, string[] commandArgs)
        {
            this.Database = database;
            this.CommandArgs = commandArgs;
        }

        public BankSystemDbContext Database { get; private set; }

        public string[] CommandArgs { get; private set; }

        public abstract string Execute();
    }
}