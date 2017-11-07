namespace _04.BankSystem.Client.Core.IO
{
    using _04.BankSystem.Models.Contracts.IO;
    using System;
    using System.Text;

    public class ConsoleWriter : IWriter
    {
        private StringBuilder sb;

        public ConsoleWriter()
        {
            this.sb = new StringBuilder();
        }

        public void WriteLine(string message)
        {
            this.sb.AppendLine(message);
        }

        public void WriteAllText()
        {
            Console.WriteLine(this.sb.ToString().Trim());
        }
    }
}