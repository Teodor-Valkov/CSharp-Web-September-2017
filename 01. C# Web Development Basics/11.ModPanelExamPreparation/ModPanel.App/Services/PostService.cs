namespace ModPanel.App.Services
{
    using AutoMapper.QueryableExtensions;
    using Data;
    using Data.Models;
    using Infrastructure.Helpers;
    using Models.Home;
    using Models.Posts;
    using Services.Contracts;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class PostService : IPostService
    {
        private ModPanelDbContext database;

        public PostService(ModPanelDbContext database)
        {
            this.database = database;
        }

        public void Create(string title, string content, int userId)
        {
            Post post = new Post
            {
                Title = title.Capitalize(),
                Content = content,
                UserId = userId,
                CreatedOn = DateTime.UtcNow
            };

            this.database.Posts.Add(post);
            this.database.SaveChanges();
        }

        public PostModel GetById(int id)
        {
            return this.database
                .Posts
                .Where(p => p.Id == id)
                .ProjectTo<PostModel>()
                .FirstOrDefault();
        }

        public void Edit(int id, string title, string content)
        {
            Post post = database.Posts.Find(id);

            if (post == null)
            {
                return;
            }

            post.Title = title.Capitalize();
            post.Content = content;

            this.database.SaveChanges();
        }

        public string Delete(int id)
        {
            Post post = database.Posts.Find(id);

            if (post == null)
            {
                return null;
            }

            this.database.Posts.Remove(post);
            this.database.SaveChanges();

            return post.Title;
        }

        public IEnumerable<PostsListModel> All()
        {
            return this.database
                .Posts
                .OrderByDescending(p => p.CreatedOn)
                .ProjectTo<PostsListModel>()
                .ToList();
        }

        public IEnumerable<HomeListModel> AllFromSearch(string search)
        {
            IQueryable<Post> query = this.database.Posts.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(p => p.Title.ToLower().Contains(search.ToLower()));
            }

            return query
                .OrderByDescending(p => p.CreatedOn)
                .ProjectTo<HomeListModel>()
                .ToList();
        }
    }
}