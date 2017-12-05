namespace LearningSystem.Web.Areas.Trainer.Models.Courses
{
    using Services.Models.Courses;
    using Services.Trainer.Models.Users;
    using System.Collections.Generic;

    public class CourseWithStudentsViewModel
    {
        public IEnumerable<StudentInCourseServiceModel> Students { get; set; }

        public CourseBasicServiceModel Course { get; set; }
    }
}