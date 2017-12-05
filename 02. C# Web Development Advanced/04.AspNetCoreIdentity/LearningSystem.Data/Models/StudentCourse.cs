namespace LearningSystem.Data.Models
{
    using Enums;
    using System.ComponentModel.DataAnnotations;

    using static DataConstants;

    public class StudentCourse
    {
        public string StudentId { get; set; }

        public User Student { get; set; }

        public int CourseId { get; set; }

        public Course Course { get; set; }

        public Grade? Grade { get; set; }

        [MaxLength(ExamSubmissionFileLength)]
        public byte[] ExamSubmission { get; set; }
    }
}