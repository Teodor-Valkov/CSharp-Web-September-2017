namespace _04.BankSystem.Client.Core.IO
{
    using _04.BankSystem.Models.Contracts.IO;
    using System;

    public class ConsoleReader : IReader
    {
        public string ReadLine()
        {
            return Console.ReadLine();
        }
    }
}