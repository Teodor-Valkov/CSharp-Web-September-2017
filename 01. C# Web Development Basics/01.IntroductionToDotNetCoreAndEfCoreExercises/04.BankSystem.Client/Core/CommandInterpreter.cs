namespace _04.BankSystem.Client.Core
{
    using _04.BankSystem.Data;
    using _04.BankSystem.Models.Contracts.Core;
    using System;
    using System.Linq;
    using System.Reflection;

    public class CommandInterpreter : ICommandInterpreter
    {
        private const string CommandSuffix = "Command";

        private BankSystemDbContext database;

        public CommandInterpreter(BankSystemDbContext database)
        {
            this.database = database;
        }

        public string ProcessCommand(string input)
        {
            string[] commandArgs = input.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries).ToArray();
            string commandName = commandArgs[0];
            commandArgs = commandArgs.Skip(1).ToArray();

            Type commandType = Assembly.GetExecutingAssembly().DefinedTypes.FirstOrDefault(c => c.Name == commandName + CommandSuffix);
            ICommand command = (ICommand)Activator.CreateInstance(commandType, new object[] { database, commandArgs });

            string message = command.Execute();
            return message;
        }
    }
}