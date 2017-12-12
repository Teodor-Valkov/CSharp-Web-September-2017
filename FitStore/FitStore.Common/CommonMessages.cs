﻿namespace FitStore.Common
{
    public class CommonMessages
    {
        public const string EntityNotFound = "{0} not found.";
        public const string EntityExists = "{0} with that name already exists.";
        public const string EntityCreated = "{0} *{1}* was created successfully.";
        public const string EntityModified = "{0} *{1}* was edited successfully.";
        public const string EntityNotModified = "No changes to apply.";
        public const string EntityDeleted = "{0} *{1}* was deleted successfully.";
        public const string EntityRestored = "{0} *{1}* was restored successfully.";

        public const string InvalidIdentityDetailsErroMessage = "Invalid identity details.";
        public const string ChangeRoleErrorMessage = "Error. {0}";
        public const string ChangeRoleSuccessMessage = "User *{0}* has been added to role *{1}*.";

        public const string UserEditProfileErrorMessage = "Error. Your profile could not be changed. Please try again.";
        public const string UserEditProfileSuccessMessage = "Your profile was successfully changed.";

        public const string UserChangePasswordExternalLoginErrorMessage = "You do not have a password. You are using an external login provider.";
        public const string UserChangePasswordErrorMessage = "Error. Your password could not be changed. Please try again.";
        public const string UserChangePasswordSuccessMessage = "Your password was successfully changed.";

        public const string SupplementAddedToCartSuccessMessage = "Supplement was successfully added to your cart.";
        public const string SupplementRemovedFromCartErrorMessage = "You do not have this supplement in your cart.";
        public const string SupplementRemovedFromCartSuccessMessage = "Supplement was successfully removed from your cart.";

        public const string SupplementUnavailableErrorMessage = "This supplement is currently out of stock.";

        public const string CancelOrderSuccessMessage = "Your order was successfully canceled.";
        public const string FinishOrderSuccessMessage = "Thank you for your order.";
    }
}