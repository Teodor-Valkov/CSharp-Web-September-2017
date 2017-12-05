namespace LearningSystem.Services.Admin.Models.Users
{
    using Common.Mapping;
    using Data.Models;

    public class AdminUserIdAndNameServiceModel : IMapFrom<User>
    {
        public string Id { get; set; }

        public string Name { get; set; }
    }
}