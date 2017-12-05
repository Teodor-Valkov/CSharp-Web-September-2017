namespace CameraBazaar.Web.Infrastructure.Validations
{
    using Data.Common;
    using System.ComponentModel.DataAnnotations;

    public class ImageAttribute : ValidationAttribute
    {
        public ImageAttribute()
        {
            this.ErrorMessage = PropertyErrorMessageConstants.ImageUrlContentErrorMessage;
        }

        public override bool IsValid(object value)
        {
            string imageUrl = value as string;

            if (imageUrl == null)
            {
                return true;
            }

            if (!imageUrl.StartsWith("http://") && !imageUrl.StartsWith("https://"))
            {
                return false;
            }

            return true;
        }
    }
}