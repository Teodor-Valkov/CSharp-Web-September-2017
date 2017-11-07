namespace Judge.App.Services.Contracts
{
    using Models.Contests;
    using System.Collections.Generic;

    public interface IContestService
    {
        void Create(string name, int userId);

        void Edit(int id, string name);

        void Delete(int id);

        int GetContestAuthorId(int id);

        ContestModel GetById(int id);

        IList<string> AllNames();

        IEnumerable<ContestListModel> All();

        IEnumerable<ContestByIdModel> AllContestsById();
    }
}