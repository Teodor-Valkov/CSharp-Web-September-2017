namespace ModPanel.App.Services.Contracts
{
    using Data.Models.Enums;
    using ModPanel.App.Models.Admin;
    using System.Collections.Generic;

    public interface IUserService
    {
        bool Create(string email, string password, PositionType position);

        bool UserExists(string email, string password);

        bool UserIsApproved(string email);

        IEnumerable<AdminUsersListModel> All();

        string Approve(int id);
    }
}