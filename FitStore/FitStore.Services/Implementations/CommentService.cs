namespace FitStore.Services.Implementations
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoMapper.QueryableExtensions;
    using FitStore.Data;
    using FitStore.Data.Models;
    using FitStore.Services.Contracts;
    using FitStore.Services.Models.Comments;
    using Microsoft.EntityFrameworkCore;

    public class CommentService : ICommentService
    {
        private readonly FitStoreDbContext database;

        public CommentService(FitStoreDbContext database)
        {
            this.database = database;
        }

        public async Task CreateAsync(string content, string authorId, int supplementId)
        {
            Comment comment = new Comment
            {
                Content = content,
                PublishDate = DateTime.UtcNow,
                AuthorId = authorId,
                SupplementId = supplementId
            };

            await this.database.Comments.AddAsync(comment);
            await this.database.SaveChangesAsync();
        }

        public async Task<CommentBasicServiceModel> GetEditModelAsync(int commentId)
        {
            return await this.database
                .Comments
                .Where(c => c.Id == commentId)
                .ProjectTo<CommentBasicServiceModel>()
                .FirstOrDefaultAsync();
        }

        public async Task EditAsync(int commentId, string content)
        {
            Comment comment = await this.database
                .Comments
                .Where(c => c.Id == commentId)
                .FirstOrDefaultAsync();

            comment.Content = content;

            this.database.Comments.Update(comment);
            await this.database.SaveChangesAsync();
        }

        public async Task<bool> IsCommentModified(int commentId, string content)
        {
            Comment comment = await this.database
                .Comments
                .Where(c => c.Id == commentId)
                .FirstOrDefaultAsync();

            if (content == comment.Content)
            {
                return false;
            }

            return true;
        }

        public async Task DeleteAsync(int commentId)
        {
            Comment comment = await this.database
                .Comments
                .Where(c => c.Id == commentId)
                .FirstOrDefaultAsync();

            comment.IsDeleted = true;

            await this.database.SaveChangesAsync();
        }

        public Task<bool> IsUserAuthor(int commentId, string authorId)
        {
            return this.database
                .Comments
                .AnyAsync(c => c.Id == commentId && c.AuthorId == authorId);
        }

        public Task<bool> IsCommentExistingById(int commentId, bool isDeleted)
        {
            return this.database
                .Comments
                .AnyAsync(c => c.Id == commentId && c.IsDeleted == isDeleted);
        }
    }
}