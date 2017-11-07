namespace _02.SocialNetwork.Models.Attributes
{
    using System;
    using System.ComponentModel.DataAnnotations;

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class AgeAttribute : ValidationAttribute
    {
        private const int MinAge = 1;
        private const int MaxAge = 120;

        public AgeAttribute()
        {
            this.ErrorMessage = "Age should be between 1 and 120 years!";
        }

        public override bool IsValid(object value)
        {
            string ageAsString = value.ToString();

            if (ageAsString == null)
            {
                return true;
            }

            int age = int.Parse(ageAsString);

            if (age < MinAge || age > MaxAge)
            {
                return false;
            }

            return true;
        }
    }
}