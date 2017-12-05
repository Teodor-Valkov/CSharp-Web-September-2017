namespace CarDealer.Services.Models.Logs
{
    using Data.Models.Enums;
    using System;

    public class LogListServiceModel
    {
        public string UserName { get; set; }

        public Operation Operation { get; set; }

        public ModifiedTable ModifiedTable { get; set; }

        public DateTime ModifiedOn { get; set; }
    }
}