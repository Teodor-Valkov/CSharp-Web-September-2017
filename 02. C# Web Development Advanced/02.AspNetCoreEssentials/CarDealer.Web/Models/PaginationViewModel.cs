namespace CarDealer.Web.Models
{
    public class PaginationViewModel
    {
        public int CurrentPage { get; set; }

        public int PreviousPage { get; set; }

        public int NextPage { get; set; }

        public int TotalPages { get; set; }

        public string ActionLink { get; set; }
    }
}