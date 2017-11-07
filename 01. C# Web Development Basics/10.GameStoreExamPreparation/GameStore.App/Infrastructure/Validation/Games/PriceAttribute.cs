namespace GameStore.App.Infrastructure.Validation.Games
{
    using SimpleMvc.Framework.Attributes.Validation;

    public class PriceAttribute : PropertyValidationAttribute
    {
        public override bool IsValid(object value)
        {
            //string priceAsString = value as string;

            string priceAsString = value.ToString();

            if (priceAsString == null)
            {
                return true;
            }

            return decimal.Parse(priceAsString) >= 0 && decimal.Parse(priceAsString) <= decimal.MaxValue;
        }
    }
}