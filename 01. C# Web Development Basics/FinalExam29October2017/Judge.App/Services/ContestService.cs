namespace Judge.App.Services
{
    using AutoMapper.QueryableExtensions;
    using Data;
    using Data.Models;
    using Models.Contests;
    using Services.Contracts;
    using System.Collections.Generic;
    using System.Linq;

    public class ContestService : IContestService
    {
        private readonly JudgeDbFinalExam database;

        public ContestService(JudgeDbFinalExam database)
        {
            this.database = database;
        }

        public void Create(string name, int userId)
        {
            Contest contest = new Contest
            {
                Name = name,
                UserId = userId
            };

            this.database.Contests.Add(contest);
            this.database.SaveChanges();
        }

        public ContestModel GetById(int id)
        {
            return this.database
                .Contests
                .Where(c => c.Id == id)
                .ProjectTo<ContestModel>()
                .FirstOrDefault();
        }

        public void Edit(int id, string name)
        {
            Contest contest = database.Contests.Find(id);

            if (contest == null)
            {
                return;
            }

            contest.Name = name;

            this.database.SaveChanges();
        }

        public void Delete(int id)
        {
            Contest contest = database.Contests.Find(id);

            if (contest == null)
            {
                return;
            }

            this.database.Contests.Remove(contest);
            this.database.SaveChanges();
        }

        public int GetContestAuthorId(int id)
        {
            return this.database
               .Contests
               .Where(c => c.Id == id)
               .FirstOrDefault()
               .UserId;
        }

        public IList<string> AllNames()
        {
            return this.database
             .Contests
             .Select(c => c.Name)
             .ToList();
        }

        public IEnumerable<ContestListModel> All()
        {
            return this.database
             .Contests
             .ProjectTo<ContestListModel>()
             .ToList();
        }

        public IEnumerable<ContestByIdModel> AllContestsById()
        {
            return this.database
                .Contests
                .ProjectTo<ContestByIdModel>()
                .ToList();
        }
    }
}