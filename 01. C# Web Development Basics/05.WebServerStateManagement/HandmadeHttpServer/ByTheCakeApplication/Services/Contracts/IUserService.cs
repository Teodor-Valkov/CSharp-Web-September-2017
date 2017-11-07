namespace HandmadeHttpServer.ByTheCakeApplication.Services.Contracts
{
    using HandmadeHttpServer.ByTheCakeApplication.ViewModels.Account;

    public interface IUserService
    {
        bool CreateUser(string fullName, string username, string password);

        bool FindUser(string username, string password);

        ProfileViewModel GetProfile(string username);

        int? GetUserId(string username);
    }
}