namespace _04.BankSystem.Client.Core
{
    using _04.BankSystem.Models.Contracts.Core;
    using _04.BankSystem.Models.Contracts.IO;
    using System;

    public class Engine : IEngine
    {
        private const string InputEndCommand = "Exit";

        private ICommandInterpreter commandInterpreter;
        private IReader reader;
        private IWriter writer;
        private bool isRunning;

        public Engine(ICommandInterpreter commandInterpreter, IReader reader, IWriter writer)
        {
            this.commandInterpreter = commandInterpreter;
            this.reader = reader;
            this.writer = writer;
            this.isRunning = true;
        }

        public void Run()
        {
            while (this.isRunning)
            {
                try
                {
                    string input = this.reader.ReadLine();

                    if (input == InputEndCommand)
                    {
                        this.isRunning = false;
                        continue;
                    }

                    string message = this.commandInterpreter.ProcessCommand(input);

                    this.writer.WriteLine(message);
                }
                catch (Exception exception)
                {
                    this.writer.WriteLine(exception.Message);
                }
            }

            this.writer.WriteAllText();
        }
    }
}