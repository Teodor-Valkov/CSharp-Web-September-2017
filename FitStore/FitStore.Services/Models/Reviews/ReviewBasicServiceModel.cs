namespace FitStore.Services.Models.Reviews
{
    using Common.Mapping;
    using Data.Models;

    public class ReviewBasicServiceModel : IMapFrom<Review>
    {
        public int Id { get; set; }

        public string Content { get; set; }

        public int Rating { get; set; }
    }
}