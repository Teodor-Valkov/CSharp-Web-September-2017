namespace GameStore.App.Services.Contracts
{
    public interface IUserService
    {
        bool RegisterUser(string fullName, string email, string password);

        bool IsUserExisting(string email, string password);

        int? GetUserId(string username);
    }
}