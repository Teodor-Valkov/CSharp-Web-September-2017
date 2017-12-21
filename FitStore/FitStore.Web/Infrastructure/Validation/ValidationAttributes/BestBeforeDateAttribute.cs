namespace FitStore.Web.Infrastructure.Validation.ValidationAttributes
{
    using System;
    using System.ComponentModel.DataAnnotations;

    using static Common.CommonMessages;

    public class BestBeforeDateAttribute : ValidationAttribute
    {
        public BestBeforeDateAttribute()
        {
            this.ErrorMessage = SupplementBestBeforeDateErrorMessage;
        }

        public override bool IsValid(object value)
        {
            DateTime? date = value as DateTime?;

            if (date == null)
            {
                return true;
            }

            if (date.Value.Date < DateTime.UtcNow)
            {
                return false;
            }

            return true;
        }
    }
}