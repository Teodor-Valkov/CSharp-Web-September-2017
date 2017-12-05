namespace CarDealer.Services.Implementations
{
    using Data;
    using Data.Models;
    using Data.Models.Enums;
    using Microsoft.EntityFrameworkCore;
    using Services.Contracts;
    using Services.Models.Logs;
    using System.Collections.Generic;
    using System.Linq;
    using System;

    public class LogService : ILogService
    {
        private CarDealerDbContext database;

        public LogService(CarDealerDbContext database)
        {
            this.database = database;
        }

        public IEnumerable<LogListServiceModel> GetAllListing(string username, int page = 1, int pageSize = 10)
        {
            IEnumerable<Log> logs = this.database.Logs.Include(l => l.User).AsQueryable();

            if (!string.IsNullOrEmpty(username))
            {
                logs = logs.Where(l => l.User.UserName.ToLower().Contains(username.ToLower())).ToList();
            }

            return logs
               .OrderBy(l => l.ModifiedOn)
               .Skip((page - 1) * pageSize)
               .Take(pageSize)
               .Select(l => new LogListServiceModel
               {
                   UserName = l.User.UserName,
                   Operation = l.Operation,
                   ModifiedTable = l.ModifiedTable,
                   ModifiedOn = l.ModifiedOn
               })
               .ToList();
        }

        public void Create(string username, Operation operation, ModifiedTable modifiedTable, DateTime date)
        {
            string userId = this.database.Users.FirstOrDefault(u => u.UserName == username).Id;

            if (string.IsNullOrEmpty(userId))
            {
                return;
            }

            Log log = new Log
            {
                UserId = userId,
                Operation = operation,
                ModifiedTable = modifiedTable,
                ModifiedOn = date
            };

            this.database.Logs.Add(log);
            this.database.SaveChanges();
        }

        public void Clear()
        {
            IEnumerable<Log> logs = this.database.Logs;

            this.database.RemoveRange(logs);
            this.database.SaveChanges();
        }

        public int TotalLogsCount(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                return this.database.Logs.Count();
            }

            return this.database.Logs.Where(l => l.User.UserName.ToLower().Contains(username.ToLower())).Count();
        }
    }
}