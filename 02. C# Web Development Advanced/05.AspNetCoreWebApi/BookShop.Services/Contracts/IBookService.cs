namespace BookShop.Services.Contracts
{
    using Models.Books;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IBookService
    {
        Task<IEnumerable<BookBasicServiceModel>> GetAllListingAsync(string searchToken);

        Task<BookDetailsServiceModel> GetBookDetailsByIdAsync(int bookId);

        Task<int> CreateAsync(
            string title,
            string description,
            decimal price,
            int copies,
            int? edition,
            int? ageRestriction,
            DateTime releaseDate,
            int authorId,
            string categories);

        Task<int> EditAsync(
            int bookId,
            string title,
            string description,
            decimal price,
            int copies,
            int? edition,
            int? ageRestriction,
            DateTime releaseDate,
            int authorId,
            string categories);

        Task<int> DeleteAsync(int bookId);
    }
}