namespace FitStore.Web.Infrastructure.Validation.ValidationAttributes
{
    using Microsoft.AspNetCore.Http;
    using System.ComponentModel.DataAnnotations;

    using static Common.CommonMessages;

    public class PictureAttribute : ValidationAttribute
    {
        public PictureAttribute()
        {
            this.ErrorMessage = SupplementPictureErrorMessage;
        }

        public override bool IsValid(object value)
        {
            IFormFile picture = value as IFormFile;

            if (picture == null)
            {
                return true;
            }

            if (!picture.FileName.EndsWith(".jpg"))
            {
                return false;
            }

            return true;
        }
    }
}