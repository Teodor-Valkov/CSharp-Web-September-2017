namespace _03.OneToOne
{
    using Data;
    using Models;

    public class StartUp
    {
        public static void Main()
        {
            using (MyDbContext context = new MyDbContext())
            {
                PrepareDatabase(context);
                AddDataToDatabase(context);
            }
        }

        private static void PrepareDatabase(MyDbContext context)
        {
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
        }

        private static void AddDataToDatabase(MyDbContext context)
        {
            Employee manager = new Employee
            {
                Name = "Manager"
            };
            Employee employee = new Employee
            {
                Name = "Employee"
            };

            employee.Manager = manager;

            context.Employees.Add(manager);
            context.Employees.Add(employee);
            context.SaveChanges();
        }
    }
}