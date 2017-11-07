namespace _04.ManyToMany
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
            Course course = new Course
            {
                Name = "C#"
            };

            context.Courses.Add(course);

            Student student = new Student
            {
                Name = "Student"
            };

            student.Courses.Add(new StudentCourse
            {
                CourseId = course.Id
            });

            context.Students.Add(student);
            context.SaveChanges();
        }
    }
}