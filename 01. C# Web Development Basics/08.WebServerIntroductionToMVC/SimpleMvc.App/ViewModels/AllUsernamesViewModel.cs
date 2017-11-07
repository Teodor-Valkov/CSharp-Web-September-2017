namespace SimpleMvc.App.ViewModels
{
    using System.Collections.Generic;

    public class AllUsersViewModel
    {
        public IEnumerable<UserViewModel> Users { get; set; }
    }
}