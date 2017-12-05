namespace CarDealer.Data.Models
{
    using Enums;
    using System;

    public class Log
    {
        public int Id { get; set; }

        public string UserId { get; set; }

        public User User { get; set; }

        public Operation Operation { get; set; }

        public ModifiedTable ModifiedTable { get; set; }

        public DateTime ModifiedOn { get; set; }
    }
}