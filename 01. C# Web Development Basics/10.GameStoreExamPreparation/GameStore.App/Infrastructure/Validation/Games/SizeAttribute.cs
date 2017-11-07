namespace GameStore.App.Infrastructure.Validation.Games
{
    using SimpleMvc.Framework.Attributes.Validation;

    public class SizeAttribute : PropertyValidationAttribute
    {
        public override bool IsValid(object value)
        {
            //string sizeAsString = value as string;

            string sizeAsString = value.ToString();

            if (sizeAsString == null)
            {
                return true;
            }

            return double.Parse(sizeAsString) >= 0 && double.Parse(sizeAsString) <= double.MaxValue;
        }
    }
}