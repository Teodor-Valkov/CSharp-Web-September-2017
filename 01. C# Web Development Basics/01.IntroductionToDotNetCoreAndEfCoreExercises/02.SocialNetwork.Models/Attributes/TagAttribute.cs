namespace _02.SocialNetwork.Models.Attributes
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Text.RegularExpressions;

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class TagAttribute : ValidationAttribute
    {
        public TagAttribute()
        {
            this.ErrorMessage = $"Tag should start with '#', should not have any whitespaces, should be between 2 and 20 symbols and contain only letters or digits!";
        }

        public override bool IsValid(object value)
        {
            string tagAsString = value as string;

            if (tagAsString == null)
            {
                return true;
            }

            Regex regex = new Regex("#[a-zA-Z0-9]{2,20}");

            if (!regex.IsMatch(tagAsString))
            {
                return false;
            }

            return true;
        }
    }
}