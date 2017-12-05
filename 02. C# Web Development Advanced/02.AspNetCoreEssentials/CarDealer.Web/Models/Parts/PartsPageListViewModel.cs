namespace CarDealer.Web.Models.Parts
{
    using Services.Models.Parts;
    using System.Collections.Generic;

    public class PartsPageListViewModel
    {
        public IEnumerable<PartListServiceModel> Parts { get; set; }

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