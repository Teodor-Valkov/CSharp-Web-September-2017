namespace ModPanel.App.Services.Contracts
{
    using Models.Home;
    using Models.Posts;
    using System.Collections.Generic;

    public interface IPostService
    {
        void Create(string title, string content, int userId);

        PostModel GetById(int id);

        void Edit(int id, string title, string content);

        string Delete(int id);

        IEnumerable<PostsListModel> All();

        IEnumerable<HomeListModel> AllFromSearch(string search);
    }
}