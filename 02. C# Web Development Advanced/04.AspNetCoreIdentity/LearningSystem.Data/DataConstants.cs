namespace LearningSystem.Data
{
    public static class DataConstants
    {
        public const int ArticleTitleMinLength = 3;
        public const int ArticleTitleMaxLength = 50;

        public const int CourseNameMaxLength = 50;
        public const int CourseDescriptionMaxLength = 2000;

        public const int UserNameMinLength = 2;
        public const int UserNameMaxLength = 100;

        public const int UserUsernameMinLength = 2;
        public const int UserUsernameMaxLength = 100;

        public const int UserEmailMinLength = 6;
        public const int UserEmailMaxLength = 100;

        public const int UserPasswordMinLength = 6;
        public const int UserPasswordMaxLength = 100;

        public const int ExamSubmissionFileLength = 2 * 1024 * 1024;
    }
}