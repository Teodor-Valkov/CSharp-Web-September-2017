namespace _01.StudentSystem.Client
{
    using _01.StudentSystem.Data;
    using _01.StudentSystem.Models;
    using _01.StudentSystem.Models.Enums;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class StartUp
    {
        private static Random random = new Random();

        public static void Main()
        {
            using (StudentSystemDbContext database = new StudentSystemDbContext())
            {
                // Printing exercises are in class HelperMethods.cs

                database.Database.Migrate();

                //SeedDatabase(database);
                //SeedLicenses(database);

                //HelperMethods.PrintStudentsWithHomeworks(database);
                //HelperMethods.PrintCoursesAndResources(database);
                //HelperMethods.PrintCoursesWithMoreThanFiveResources(database);
                //HelperMethods.PrintCoursesActiveOnGivenDate(database);
                //HelperMethods.PrinStudentsWithPrices(database);

                //HelperMethods.PrintCoursesWithResourcesAndLicences(database);
                //HelperMethods.PrintStudentsWithCoursesResourcesAndLicences(database);
            }
        }

        private static void SeedDatabase(StudentSystemDbContext database)
        {
            const int totalStudents = 25;
            const int totalCourses = 10;

            DateTime currentDate = DateTime.Now;

            //Students
            for (int i = 0; i < totalStudents; i++)
            {
                Student student = new Student
                {
                    Name = $"Student {i}",
                    RegistrationDate = currentDate.AddDays(i),
                    Birthday = currentDate.AddYears(-20).AddDays(i),
                    Phone = $"Random Phone {i}"
                };

                database.Students.Add(student);
            }

            database.SaveChanges();

            List<Course> addedCourses = new List<Course>();

            //Courses
            for (int i = 0; i < totalCourses; i++)
            {
                Course course = new Course
                {
                    Name = $"Course {i}",
                    Description = $"Course Details {i}",
                    Price = i * 100,
                    StartDate = currentDate.AddDays(i),
                    EndDate = currentDate.AddDays(i + 20)
                };

                addedCourses.Add(course);
                database.Courses.Add(course);
            }

            database.SaveChanges();

            List<int> studentIds = database.Students.Select(s => s.Id).ToList();

            //Students in Courses
            for (int i = 0; i < totalCourses; i++)
            {
                Course currentCourse = addedCourses[i];
                int studentsInCourse = random.Next(2, totalStudents / 2);

                for (int j = 0; j < studentsInCourse; j++)
                {
                    int studentId = studentIds[random.Next(1, studentIds.Count)];

                    if (!currentCourse.Students.Any(s => s.StudentId == studentId))
                    {
                        currentCourse.Students.Add(new StudentCourse
                        {
                            StudentId = studentId
                        });
                    }
                    else
                    {
                        j--;
                    }
                }

                //Resources
                int resourcesInCourse = random.Next(2, 20);
                int[] resourceTypes = new[] { 0, 1, 2, 999 };

                for (int j = 0; j < resourcesInCourse; j++)
                {
                    Resource resource = new Resource
                    {
                        Name = $"Resource {i} {j}",
                        Url = $"URL {i} {j}",
                        Type = (ResourceType)resourceTypes[random.Next(0, resourceTypes.Length)]
                    };

                    currentCourse.Resources.Add(resource);
                }
            }

            database.SaveChanges();

            //Homeworks
            for (int i = 0; i < totalCourses; i++)
            {
                Course currentCourse = addedCourses[i];

                List<int> studentsInCurrentCourseIds = currentCourse.Students.Select(s => s.StudentId).ToList();

                for (int j = 0; j < studentsInCurrentCourseIds.Count; j++)
                {
                    int totalHomeworks = random.Next(2, 5);

                    for (int k = 0; k < totalHomeworks; k++)
                    {
                        Homework homework = new Homework
                        {
                            Content = $"Content Homework {i}",
                            SubmissionDate = currentDate.AddDays(-1),
                            Type = ContentType.Zip,
                            StudentId = studentsInCurrentCourseIds[j],
                            CourseId = currentCourse.Id
                        };

                        database.Homeworks.Add(homework);
                    }
                }
            }

            database.SaveChanges();
        }

        private static void SeedLicenses(StudentSystemDbContext database)
        {
            //Licenses
            List<int> resourcesIds = database.Resources.Select(r => r.Id).ToList();

            for (int i = 0; i < resourcesIds.Count; i++)
            {
                int totalLicenses = random.Next(2, 5);

                for (int j = 0; j < totalLicenses; j++)
                {
                    License license = new License
                    {
                        Name = $"License {i} {j}",
                        ResourceId = resourcesIds[i]
                    };

                    database.Licenses.Add(license);
                }
            }

            database.SaveChanges();
        }
    }
}