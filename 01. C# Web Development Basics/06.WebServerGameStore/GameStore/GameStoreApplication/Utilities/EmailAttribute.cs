namespace GameStore.GameStoreApplication.Utilities
{
    using System.ComponentModel.DataAnnotations;

    public class EmailAttribute : ValidationAttribute
    {
        public EmailAttribute()
        {
            this.ErrorMessage = "Email must contain '@' sign and a '.'!";
        }

        public override bool IsValid(object value)
        {
            string email = value as string;

            if (email == null)
            {
                return true;
            }

            return email.Contains("@") && email.Contains(".");
        }
    }
}