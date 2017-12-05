namespace LearningSystem.Services.Blog.Implementations
{
    using AutoMapper.QueryableExtensions;
    using Contracts;
    using Data;
    using Data.Models;
    using Microsoft.EntityFrameworkCore;
    using Models.Articles;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using static Common.CommonConstants;

    public class BlogArticleService : IBlogArticleService
    {
        private readonly LearningSystemDbContext database;

        public BlogArticleService(LearningSystemDbContext database)
        {
            this.database = database;
        }

        public async Task<IEnumerable<ArticleBasicServiceModel>> GetAllListingAsync(string searchToken, int page)
        {
            IQueryable<Article> articles = this.database.Articles;

            if (!string.IsNullOrWhiteSpace(searchToken))
            {
                articles = articles.Where(a =>
                    a.Title.ToLower().Contains(searchToken.ToLower()) ||
                    a.Content.ToLower().Contains(searchToken.ToLower()));
            }

            return await articles
               .OrderByDescending(a => a.PublishDate)
               .Skip((page - 1) * ArticlePageSize)
               .Take(ArticlePageSize)
               .ProjectTo<ArticleBasicServiceModel>()
               .ToListAsync();
        }

        public async Task<IEnumerable<ArticleSearchServiceModel>> GetAllSearchListingAsync(string searchToken)
        {
            IQueryable<Article> articles = this.database.Articles;

            if (!string.IsNullOrWhiteSpace(searchToken))
            {
                articles = articles.Where(a =>
                    a.Title.ToLower().Contains(searchToken.ToLower()) ||
                    a.Content.ToLower().Contains(searchToken.ToLower()));
            }

            return await articles
               .OrderByDescending(a => a.PublishDate)
               .ProjectTo<ArticleSearchServiceModel>()
               .ToListAsync();
        }

        public async Task<ArticleDetailsServiceModel> GetDetailsByIdAsync(int id)
        {
            return await this.database
                .Articles
                .Where(a => a.Id == id)
                .ProjectTo<ArticleDetailsServiceModel>()
                .FirstOrDefaultAsync();
        }

        public async Task CreateAsync(string title, string content, DateTime publishDate, string authorId)
        {
            Article article = new Article
            {
                Title = title,
                Content = content,
                PublishDate = publishDate,
                AuthorId = authorId
            };

            await this.database.Articles.AddAsync(article);
            await this.database.SaveChangesAsync();
        }

        public async Task<int> TotalCountAsync(string searchToken)
        {
            if (string.IsNullOrWhiteSpace(searchToken))
            {
                return await this.database.Articles.CountAsync();
            }

            return await this.database
              .Articles
              .Where(a => a.Title.ToLower().Contains(searchToken.ToLower()))
              .CountAsync();
        }
    }
}