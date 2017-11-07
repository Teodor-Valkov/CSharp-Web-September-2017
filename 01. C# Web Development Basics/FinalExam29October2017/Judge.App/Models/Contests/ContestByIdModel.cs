namespace Judge.App.Models.Contests
{
    using Data.Models;
    using Infrastructure.Mapping;

    public class ContestByIdModel : IMapFrom<Contest>
    {
        public int Id { get; set; }

        public string Name { get; set; }
    }
}