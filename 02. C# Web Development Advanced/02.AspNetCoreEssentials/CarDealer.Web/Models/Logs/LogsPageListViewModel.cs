namespace CarDealer.Web.Models.Logs
{
    using Services.Models.Logs;
    using System.Collections.Generic;

    public class LogsPageListViewModel
    {
        public IEnumerable<LogListServiceModel> Logs { get; set; }

        public string Username { get; set; }

        public int CurrentPage { get; set; }

        public int TotalPages { get; set; }

        public int PreviousPage
        {
            get
            {
                return this.CurrentPage == 1
                    ? 1
                    : this.CurrentPage - 1;
            }
        }

        public int NextPage
        {
            get
            {
                return this.CurrentPage == this.TotalPages
                    ? this.TotalPages
                    : this.CurrentPage + 1;
            }
        }
    }
}