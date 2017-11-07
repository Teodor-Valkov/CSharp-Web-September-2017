namespace ModPanel.App.Services
{
    using AutoMapper.QueryableExtensions;
    using Data;
    using Data.Models;
    using Data.Models.Enums;
    using Models.Logs;
    using Services.Contracts;
    using System.Collections.Generic;
    using System.Linq;

    public class LogService : ILogService
    {
        private readonly ModPanelDbContext database;

        public LogService(ModPanelDbContext database)
        {
            this.database = database;
        }

        public void Create(string admin, LogType type, string additionalInformation)
        {
            Log log = new Log
            {
                Admin = admin,
                Type = type,
                AdditionalInformation = additionalInformation
            };

            this.database.Logs.Add(log);
            this.database.SaveChanges();
        }

        public IEnumerable<LogModel> All()
        {
            return this.database.Logs
                .OrderByDescending(l => l.Id)
                .ProjectTo<LogModel>()
                .ToList();
        }
    }
}