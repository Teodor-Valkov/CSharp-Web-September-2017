namespace FitStore.Common
{
    public class CommonMessages
    {
        public const string EntityNotFound = "{0} not found.";
        public const string EntityExists = "{0} with that name already exists.";
        public const string EntityNotExists = " {0} does not exist.";
        public const string EntityCreated = "{0} was created successfully.";
        public const string EntityModified = "{0} was edited successfully.";
        public const string EntityNotModified = "No changes to apply.";
        public const string EntityDeleted = "{0} was deleted successfully.";
        public const string EntityRestored = "{0} was restored successfully.";
        public const string EntityNotRestored = "{0} cannot be restored.";

        public const string InvalidIdentityDetailsErroMessage = "Invalid identity details.";
        public const string ChangeRoleErrorMessage = "Error. {0}";
        public const string AddToRoleSuccessMessage = "User {0} has been added to role {1}.";
        public const string RemoveFromRoleSuccessMessage = "User {0} has been removed from role {1}.";

        public const string UserEditProfileSuccessMessage = "Your profile was successfully changed.";

        public const string UserOldPasswordNotValid = "Current password is not valid.";
        public const string UserChangePasswordExternalLoginErrorMessage = "You do not have a password. You are using an external login provider.";
        public const string UserChangePasswordErrorMessage = "Error. Your password could not be changed. Please try again.";
        public const string UserChangePasswordSuccessMessage = "Your password was successfully changed.";

        public const string SupplementAddedToCartSuccessMessage = "Supplement was successfully added to your cart.";
        public const string SupplementCannotBeRemovedFromCartErrorMessage = "You do not have this supplement in your cart.";
        public const string SupplementRemovedFromCartSuccessMessage = "Supplement was successfully removed from your cart.";

        public const string SupplementUnavailableErrorMessage = "This supplement is currently out of stock.";
        public const string SupplementLastOneJustAddedErrorMessage = "You have already added the last available supplement in your cart.";

        public const string CancelOrderSuccessMessage = "Your order was successfully canceled.";
        public const string FinishOrderSuccessMessage = "Thank you for your order.";
        public const string FinishOrderErrorMessage = "Oops! Someone was faster than you... Try to add less supplement quantities in your cart.";

        public const string UserRestrictedErrorMessage = "You are temporarily restricted to perform this action.";

        public const string SupplementPictureErrorMessage = "Only pictures in Jpeg format are allowed.";
        public const string SupplementBestBeforeDateErrorMessage = "Best before date should be in the future.";
        public const string SupplementPriceErrorMessage = "Price should be positive number.";
        public const string SupplementQuantityErrorMessage = "Quantity should be positive number.";

        public const string PasswordsDoNotMatch = "The Password and Confirmation password do not match.";
        public const string FieldLengthErrorMessage = "The {0} must be at least {2} and less than {1} characters long.";
    }
}