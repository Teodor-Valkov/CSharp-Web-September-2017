namespace GameStore.App.Infrastructure.Validation.Games
{
    using SimpleMvc.Framework.Attributes.Validation;
    using System.Linq;

    public class TitleAttribute : PropertyValidationAttribute
    {
        public override bool IsValid(object value)
        {
            string title = value as string;

            if (title == null)
            {
                return true;
            }

            return title.Length >= 3
                && title.Length <= 100
                && char.IsUpper(title.First());
        }
    }
}