namespace BookShop.Services.Implementations
{
    using AutoMapper.QueryableExtensions;
    using Common.Extensions;
    using Contracts;
    using Data;
    using Microsoft.EntityFrameworkCore;
    using Models.Books;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System;
    using BookShop.Data.Models;

    public class BookService : IBookService
    {
        private readonly BookShopDbContext database;

        public BookService(BookShopDbContext database)
        {
            this.database = database;
        }

        public async Task<IEnumerable<BookBasicServiceModel>> GetAllListingAsync(string searchToken)
        {
            return await this.database
                .Books
                .Where(b => b.Title.ToLower().Contains(searchToken.ToLower()))
                .OrderBy(b => b.Title)
                .Take(10)
                .ProjectTo<BookBasicServiceModel>()
                .ToListAsync();
        }

        public async Task<BookDetailsServiceModel> GetBookDetailsByIdAsync(int bookId)
        {
            return await this.database
                .Books
                .Where(b => b.Id == bookId)
                .ProjectTo<BookDetailsServiceModel>()
                .FirstOrDefaultAsync();
        }

        public async Task<int> CreateAsync(string title, string description, decimal price, int copies, int? edition, int? ageRestriction, DateTime releaseDate, int authorId, string categories)
        {
            ISet<string> categoryNames = categories.Split(new[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries).ToHashSet();

            IEnumerable<Category> existingCategories = await this.database
                .Categories
                .Where(c => categoryNames.Contains(c.Name))
                .ToListAsync();

            IList<Category> allCategories = new List<Category>(existingCategories);

            foreach (string categoryName in categoryNames)
            {
                if (existingCategories.All(c => c.Name != categoryName))
                {
                    Category category = new Category
                    {
                        Name = categoryName
                    };

                    this.database.Categories.Add(category);

                    allCategories.Add(category);
                }
            }

            await this.database.SaveChangesAsync();

            Book book = new Book
            {
                Title = title,
                Description = description,
                Price = price,
                Copies = copies,
                Edition = edition,
                AgeRestriction = ageRestriction,
                ReleaseDate = releaseDate,
                AuthorId = authorId
            };

            foreach (Category category in allCategories)
            {
                BookCategory bookCategory = new BookCategory
                {
                    BookId = book.Id,
                    CategoryId = category.Id
                };

                book.Categories.Add(bookCategory);
            }

            await this.database.Books.AddAsync(book);
            await this.database.SaveChangesAsync();

            return book.Id;
        }

        public async Task<int> EditAsync(int bookId, string title, string description, decimal price, int copies, int? edition, int? ageRestriction, DateTime releaseDate, int authorId, string categories)
        {
            ISet<string> categoryNames = categories.Split(new[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries).ToHashSet();

            IEnumerable<Category> existingCategories = await this.database
                .Categories
                .Where(c => categoryNames.Contains(c.Name))
                .ToListAsync();

            IList<Category> allCategories = new List<Category>(existingCategories);

            foreach (string categoryName in categoryNames)
            {
                if (existingCategories.All(c => c.Name != categoryName))
                {
                    Category category = new Category
                    {
                        Name = categoryName
                    };

                    this.database.Categories.Add(category);

                    allCategories.Add(category);
                }
            }

            await this.database.SaveChangesAsync();

            Book book = await this.database
                .Books
                .Include(b => b.Categories)
                .ThenInclude(c => c.Category)
                .Where(b => b.Id == bookId)
                .FirstOrDefaultAsync();

            if (book == null)
            {
                return 0;
            }

            book.Title = title;
            book.Description = description;
            book.Price = price;
            book.Copies = copies;
            book.Edition = edition;
            book.AgeRestriction = ageRestriction;
            book.ReleaseDate = releaseDate;
            book.AuthorId = authorId;

            foreach (Category category in allCategories)
            {
                if (book.Categories.All(c => c.Category.Name != category.Name))
                {
                    BookCategory bookCategory = new BookCategory
                    {
                        BookId = book.Id,
                        CategoryId = category.Id
                    };

                    book.Categories.Add(bookCategory);
                }
            }

            this.database.Books.Update(book);
            await this.database.SaveChangesAsync();

            return book.Id;
        }

        public async Task<int> DeleteAsync(int bookId)
        {
            Book book = await this.database
                .Books
                .Where(b => b.Id == bookId)
                .FirstOrDefaultAsync();

            if (book == null)
            {
                return 0;
            }

            this.database.Books.Remove(book);
            await this.database.SaveChangesAsync();

            return book.Id;
        }
    }
}