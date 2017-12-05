namespace LearningSystem.Services.Trainer.Contracts
{
    using Data.Models.Enums;
    using Models.Users;
    using Models.Courses;
    using Services.Models.Courses;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface ITrainerService
    {
        Task<IEnumerable<CourseBasicServiceModel>> GetAllListingByTrainerIdAsync(string trainerId, string searchToken, int page);

        Task<IEnumerable<StudentInCourseServiceModel>> GetStudentsInCourseByCourseIdAsync(int courseId);

        Task<bool> IsUserTrainerAsync(int courseId, string trainerId);

        Task<bool> AssessStudentAsync(int courseId, string studentId, Grade grade);

        Task<byte[]> DownloadExamSubmission(int courseId, string studentId);

        Task<CourseNameWithStudentNameServiceModel> GetCourseNameAndStudentName(int courseId, string studentId);

        Task<int> TotalCountAsync(string trainerId, string searchToken);
    }
}