namespace CarDealer.Services.Contracts
{
    using Data.Models.Enums;
    using Services.Models.Logs;
    using System.Collections.Generic;
    using System;

    public interface ILogService
    {
        IEnumerable<LogListServiceModel> GetAllListing(string username, int page = 1, int pageSize = 10);

        void Create(string username, Operation operation, ModifiedTable modifiedTable, DateTime date);

        void Clear();

        int TotalLogsCount(string username);
    }
}