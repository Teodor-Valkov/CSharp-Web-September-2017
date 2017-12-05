namespace FitStore.Services.Admin.Models.Users
{
    using Common.Mapping;
    using Data.Models;

    public class AdminUserBasicServiceModel : IMapFrom<User>
    {
        public string Id { get; set; }

        public string Username { get; set; }

        public string FullName { get; set; }

        public string Email { get; set; }
    }
}