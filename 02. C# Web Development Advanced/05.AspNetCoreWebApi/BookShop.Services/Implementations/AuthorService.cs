namespace BookShop.Services.Implementations
{
    using AutoMapper.QueryableExtensions;
    using Contracts;
    using Data;
    using Data.Models;
    using Microsoft.EntityFrameworkCore;
    using Models.Authors;
    using Models.Books;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class AuthorService : IAuthorService
    {
        private readonly BookShopDbContext database;

        public AuthorService(BookShopDbContext database)
        {
            this.database = database;
        }

        public async Task<AuthorDetailsServiceModel> GetAuthorDetailsByIdAsync(int authorId)
        {
            return await this.database
                .Authors
                .Where(a => a.Id == authorId)
                .ProjectTo<AuthorDetailsServiceModel>()
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<BookWithCategoriesServiceModel>> GetBooksWithCategoriesByAuthorIdAsync(int authorId)
        {
            return await this.database
                .Books
                .Where(b => b.AuthorId == authorId)
                .ProjectTo<BookWithCategoriesServiceModel>()
                .ToListAsync();
        }

        public async Task<int> CreateAsync(string firstName, string lastName)
        {
            Author author = new Author
            {
                FirstName = firstName,
                LastName = lastName
            };

            await this.database.Authors.AddAsync(author);
            await this.database.SaveChangesAsync();

            return author.Id;
        }

        public async Task<bool> IsAuthorExistingAsync(int authorId)
        {
            return await this.database.Authors.AnyAsync(a => a.Id == authorId);
        }
    }
}