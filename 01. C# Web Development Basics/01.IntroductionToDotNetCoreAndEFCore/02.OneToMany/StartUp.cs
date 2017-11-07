namespace _02.OneToMany
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
            Employee employee = new Employee
            {
                Name = "Employee"
            };

            context.Employees.Add(employee);

            Department department = new Department
            {
                Name = "C#"
            };

            department.Employees.Add(employee);

            context.Departments.Add(department);
            context.SaveChanges();
        }
    }
}