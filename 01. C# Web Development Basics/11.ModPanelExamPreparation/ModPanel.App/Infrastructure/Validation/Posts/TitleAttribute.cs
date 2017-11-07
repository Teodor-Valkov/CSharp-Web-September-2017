namespace ModPanel.App.Infrastructure.Validation.Posts
{
    using SimpleMvc.Framework.Attributes.Validation;

    public class TitleAttribute : PropertyValidationAttribute
    {
        public override bool IsValid(object value)
        {
            string title = value as string;

            if (title == null)
            {
                return true;
            }

            return title.Length >= 3 && title.Length <= 100;
        }
    }
}