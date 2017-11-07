namespace _01.StudentSystem.Client
{
    using _01.StudentSystem.Data;
    using System;
    using System.Linq;

    public class HelperMethods
    {
        public static void PrintStudentsWithHomeworks(StudentSystemDbContext database)
        {
            var filteredStudents = database.Students
                .Select(s => new
                {
                    Name = s.Name,
                    Homeworks = s.Homeworks.Select(h => new
                    {
                        h.Content,
                        h.Type
                    })
                })
                .ToList();

            foreach (var student in filteredStudents)
            {
                Console.WriteLine(student.Name);

                foreach (var homework in student.Homeworks)
                {
                    Console.WriteLine($"---{homework.Content} - {homework.Type}");
                }
            }
        }

        public static void PrintCoursesAndResources(StudentSystemDbContext database)
        {
            var filteredCourses = database.Courses
                .OrderBy(c => c.StartDate)
                .ThenBy(c => c.EndDate)
                .Select(c => new
                {
                    Name = c.Name,
                    Description = c.Description,
                    Resources = c.Resources.Select(r => new
                    {
                        Name = r.Name,
                        URL = r.Url,
                        Type = r.Type
                    })
                })
                .ToList();

            foreach (var course in filteredCourses)
            {
                Console.WriteLine($"{course.Name} - {course.Description}");

                foreach (var resource in course.Resources)
                {
                    Console.WriteLine($"---{resource.Name} - {resource.Type} - {resource.URL}");
                }
            }
        }

        public static void PrintCoursesWithMoreThanFiveResources(StudentSystemDbContext database)
        {
            var filteredCourses = database.Courses
                .Where(c => c.Resources.Count > 5)
                .OrderByDescending(c => c.Resources.Count)
                .ThenByDescending(c => c.StartDate)
                .Select(c => new
                {
                    Name = c.Name,
                    Resources = c.Resources.Count
                })
                .ToList();

            foreach (var course in filteredCourses)
            {
                Console.WriteLine($"---{course.Name} - {course.Resources}");
            }
        }

        public static void PrintCoursesActiveOnGivenDate(StudentSystemDbContext database)
        {
            DateTime date = DateTime.Now.AddDays(25);

            var filteredCourses = database.Courses
                .Where(c => c.StartDate < date && date < c.EndDate)
                .Select(c => new
                {
                    Name = c.Name,
                    StartDate = c.StartDate,
                    EndDate = c.EndDate,
                    Duration = c.EndDate.Subtract(c.StartDate),
                    Students = c.Students.Count
                })
                .OrderByDescending(c => c.Students)
                .ThenByDescending(c => c.Duration)
                .ToList();

            foreach (var course in filteredCourses)
            {
                Console.WriteLine($"---{course.Name}: {course.StartDate.ToShortDateString()} - {course.EndDate.ToShortDateString()}");
                Console.WriteLine($"-Duration: {course.Duration.Days}");
                Console.WriteLine($"-Students: {course.Students}");
            }
        }

        public static void PrinStudentsWithPrices(StudentSystemDbContext database)
        {
            var filteredStudents = database.Students
                .Where(s => s.Courses.Any())
                .Select(s => new
                {
                    Name = s.Name,
                    Courses = s.Courses.Count,
                    TotalPrice = s.Courses.Sum(c => c.Course.Price),
                    AveragePrice = s.Courses.Average(c => c.Course.Price)
                })
                .OrderByDescending(s => s.TotalPrice)
                .ThenByDescending(s => s.Courses)
                .ThenBy(s => s.Name)
                .ToList();

            foreach (var student in filteredStudents)
            {
                Console.WriteLine($"---{student.Name} - {student.Courses} - {student.TotalPrice} - {student.AveragePrice}");
            }
        }

        public static void PrintCoursesWithResourcesAndLicences(StudentSystemDbContext database)
        {
            var filteredCourses = database.Courses
                .OrderByDescending(c => c.Resources.Count)
                .ThenBy(c => c.Name)
                .Select(c => new
                {
                    Name = c.Name,
                    Resouces = c.Resources
                        .OrderByDescending(r => r.Licenses.Count)
                        .ThenBy(r => r.Name)
                        .Select(r => new
                        {
                            Name = r.Name,
                            Licenses = r.Licenses.Select(l => l.Name)
                        })
                })
                .ToList();

            foreach (var course in filteredCourses)
            {
                Console.WriteLine($"*{course.Name}");

                foreach (var resource in course.Resouces)
                {
                    Console.WriteLine($"---{resource.Name}");

                    foreach (var licence in resource.Licenses)
                    {
                        Console.WriteLine($"------{licence}");
                    }
                }
            }
        }

        public static void PrintStudentsWithCoursesResourcesAndLicences(StudentSystemDbContext database)
        {
            var filteredStudents = database.Students
                .Where(s => s.Courses.Any())
                .Select(s => new
                {
                    Name = s.Name,
                    Courses = s.Courses.Count,
                    Resources = s.Courses.Sum(c => c.Course.Resources.Count),
                    Licenses = s.Courses.Sum(c => c.Course.Resources.Sum(r => r.Licenses.Count()))
                })
                .ToList();

            foreach (var student in filteredStudents)
            {
                Console.WriteLine($"---{student.Name} - {student.Courses} - {student.Resources} - {student.Licenses}");
            }
        }
    }
}