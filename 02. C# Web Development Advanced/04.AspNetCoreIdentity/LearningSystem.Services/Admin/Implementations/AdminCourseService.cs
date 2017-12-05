namespace LearningSystem.Services.Admin.Implementations
{
    using Contracts;
    using Data;
    using Data.Models;
    using System;
    using System.Threading.Tasks;

    public class AdminCourseService : IAdminCourseService
    {
        private readonly LearningSystemDbContext database;

        public AdminCourseService(LearningSystemDbContext database)
        {
            this.database = database;
        }

        public async Task CreateAsync(string name, string description, DateTime startDate, DateTime endDate, string trainerId)
        {
            Course course = new Course
            {
                Name = name,
                Description = description,
                StartDate = startDate,
                EndDate = endDate.AddDays(1),
                TrainerId = trainerId
            };

            await this.database.Courses.AddAsync(course);
            await this.database.SaveChangesAsync();
        }
    }
}