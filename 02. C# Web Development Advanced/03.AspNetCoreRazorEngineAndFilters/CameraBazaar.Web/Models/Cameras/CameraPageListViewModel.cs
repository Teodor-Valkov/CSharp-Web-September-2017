namespace CameraBazaar.Web.Models.Cameras
{
    using Services.Models.Cameras;
    using System.Collections.Generic;

    public class CameraPageListViewModel
    {
        public IEnumerable<CameraListServiceModel> Cameras { get; set; }

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