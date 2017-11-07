namespace Judge.App.Models.Contests
{
    using Data.Models;
    using Infrastructure.Mapping;
    using Infrastructure.Validation;
    using Infrastructure.Validation.Contests;

    public class ContestModel : IMapFrom<Contest>
    {
        [Required]
        [Contest]
        public string Name { get; set; }
    }
}