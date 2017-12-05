namespace LearningSystem.Services.Blog.Contracts
{
    using Models.Articles;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IBlogArticleService
    {
        Task<IEnumerable<ArticleBasicServiceModel>> GetAllListingAsync(string searchToken, int page);

        Task<IEnumerable<ArticleSearchServiceModel>> GetAllSearchListingAsync(string searchToken);

        Task<ArticleDetailsServiceModel> GetDetailsByIdAsync(int id);

        Task CreateAsync(string title, string content, DateTime publishDate, string authorId);

        Task<int> TotalCountAsync(string searchToken);
    }
}