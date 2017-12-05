namespace CameraBazaar.Web.Infrastructure.Validations
{
    using Data.Common;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;

    public class ModelAttribute : ValidationAttribute
    {
        public ModelAttribute()
        {
            this.ErrorMessage = PropertyErrorMessageConstants.ModelContentErrorMessage;
        }

        public override bool IsValid(object value)
        {
            string model = value as string;

            if (model == null)
            {
                return true;
            }

            if (model.All(c => char.IsUpper(c) || char.IsLower(c) || char.IsDigit(c) || c == '-' || c == ' '))
            {
                return true;
            }

            return false;
        }
    }
}