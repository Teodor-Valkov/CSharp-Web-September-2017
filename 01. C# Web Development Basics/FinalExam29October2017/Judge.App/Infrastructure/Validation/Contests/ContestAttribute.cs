namespace Judge.App.Infrastructure.Validation.Contests
{
    using SimpleMvc.Framework.Attributes.Validation;

    public class ContestAttribute : PropertyValidationAttribute
    {
        public override bool IsValid(object value)
        {
            string contest = value as string;

            if (contest == null)
            {
                return true;
            }

            return contest.Length >= 3
                && contest.Length <= 100
                && char.IsUpper(contest[0]);
        }
    }
}