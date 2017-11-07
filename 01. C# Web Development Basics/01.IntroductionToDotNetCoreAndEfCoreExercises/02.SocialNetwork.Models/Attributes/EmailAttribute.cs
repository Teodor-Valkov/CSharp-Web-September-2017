namespace _02.SocialNetwork.Models.Attributes
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Text.RegularExpressions;

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class EmailAttribute : ValidationAttribute
    {
        public EmailAttribute()
        {
            this.ErrorMessage = "Email should be in format <user>@<host>!";
        }

        public override bool IsValid(object value)
        {
            string emailAsString = value as string;

            if (emailAsString == null)
            {
                return true;
            }

            Regex emailRegex = new Regex(@"(?<=\s|^)([A-Za-z0-9]+[_.-]?[A-Za-z0-9]+)@([A-Za-z0-9]+(?:[.-][A-Za-z0-9]+)*[.][A-Za-z]+)");

            if (!emailRegex.IsMatch(emailAsString))
            {
                return false;
            }

            return true;
        }
    }
}