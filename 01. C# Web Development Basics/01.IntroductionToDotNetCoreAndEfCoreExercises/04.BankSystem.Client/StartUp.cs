namespace _04.BankSystem.Client
{
    using _04.BankSystem.Client.Core.IO;
    using _04.BankSystem.Data;
    using _04.BankSystem.Models.Contracts.Core;
    using _04.BankSystem.Models.Contracts.IO;
    using Core;

    public class StartUp
    {
        public static void Main()
        {
            using (BankSystemDbContext database = new BankSystemDbContext())
            {
                //PrepareDatabase(database);

                IReader reader = new ConsoleReader();
                IWriter writer = new ConsoleWriter();
                ICommandInterpreter commandDispatcher = new CommandInterpreter(database);

                IEngine engine = new Engine(commandDispatcher, reader, writer);
                engine.Run();
            }
        }

        private static void PrepareDatabase(BankSystemDbContext database)
        {
            database.Database.EnsureDeleted();
            database.Database.EnsureCreated();
        }
    }
}