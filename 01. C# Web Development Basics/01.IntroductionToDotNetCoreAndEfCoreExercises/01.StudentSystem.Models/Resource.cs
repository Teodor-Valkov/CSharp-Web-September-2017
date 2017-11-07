namespace _01.StudentSystem.Models
{
    using Models.Enums;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class Resource
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public ResourceType Type { get; set; }

        [Required]
        public string Url { get; set; }

        public int CourseId { get; set; }

        public Course Course { get; set; }

        public ICollection<License> Licenses { get; set; } = new List<License>();
    }
}