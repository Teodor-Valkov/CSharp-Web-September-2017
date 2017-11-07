namespace GameStore.App.Infrastructure.Validation.Users
{
    using SimpleMvc.Framework.Attributes.Validation;

    public class FullNameAttribute : PropertyValidationAttribute
    {
        public override bool IsValid(object value)
        {
            string fullName = value as string;

            if (fullName == null)
            {
                return true;
            }

            return fullName.Length <= 100;
        }
    }
}