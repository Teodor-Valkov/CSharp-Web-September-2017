namespace HandmadeHttpServer.SurveyApplication.Models
{
    using System;
    using System.Collections.Generic;

    public class Person
    {
        public Person(string firstName, string secondName, DateTime birthdate, string gender, string status, string recommendations, IList<string> possesions)
        {
            this.FirstName = firstName;
            this.SecondName = secondName;
            this.Birthdate = birthdate;
            this.Gender = gender;
            this.Status = status;
            this.Recommendations = recommendations;
            this.Possesions = possesions;
        }

        public string FirstName { get; set; }

        public string SecondName { get; set; }

        public DateTime Birthdate { get; set; }

        public string Gender { get; set; }

        public string Status { get; set; }

        public string Recommendations { get; set; }

        public IList<string> Possesions { get; set; } = new List<string>();

        public override string ToString()
        {
            return $"{this.FirstName},{this.SecondName},{this.Birthdate.ToShortDateString()},{this.Gender},{this.Status},{this.Recommendations},{string.Join(",", this.Possesions)}";
        }
    }
}