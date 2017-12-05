namespace BookShop.Services.Contracts
{
    using Models.Authors;
    using Models.Books;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IAuthorService
    {
        Task<AuthorDetailsServiceModel> GetAuthorDetailsByIdAsync(int authorId);

        Task<IEnumerable<BookWithCategoriesServiceModel>> GetBooksWithCategoriesByAuthorIdAsync(int authorId);

        Task<int> CreateAsync(string firstName, string lastName);

        Task<bool> IsAuthorExistingAsync(int authorId);
    }
}