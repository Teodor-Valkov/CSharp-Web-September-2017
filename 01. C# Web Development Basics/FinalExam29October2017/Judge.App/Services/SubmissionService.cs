namespace Judge.App.Services
{
    using AutoMapper.QueryableExtensions;
    using Data;
    using Data.Models;
    using Data.Models.Enums;
    using Microsoft.EntityFrameworkCore;
    using Models.Submissions;
    using Services.Contracts;
    using System.Collections.Generic;
    using System.Linq;

    public class SubmissionService : ISubmissionService
    {
        private readonly JudgeDbFinalExam database;

        public SubmissionService(JudgeDbFinalExam database)
        {
            this.database = database;
        }

        public void Create(string code, string contest, int userId, BuildResultType type)
        {
            int? contestId = this.database.Contests.Where(c => c.Name == contest).FirstOrDefault().Id;

            if (contestId == null)
            {
                return;
            }

            Submission submission = new Submission
            {
                Code = code,
                ContestId = contestId.Value,
                UserId = userId,
                BuildResultType = type
            };

            this.database.Submissions.Add(submission);
            this.database.SaveChanges();
        }

        public IEnumerable<SubmissionBuildResultTypeModel> GetContestSubmissionsTypes(int contestId, int userId)
        {
            Contest contest = this.database
                .Contests
                .Include(c => c.Submissions)
                .Where(c => c.Id == contestId)
                .FirstOrDefault();

            if (contest == null)
            {
                return null;
            }

            return contest
                .Submissions
                .Where(s => s.UserId == userId)
                .AsQueryable()
                .ProjectTo<SubmissionBuildResultTypeModel>()
                .ToList();
        }
    }
}