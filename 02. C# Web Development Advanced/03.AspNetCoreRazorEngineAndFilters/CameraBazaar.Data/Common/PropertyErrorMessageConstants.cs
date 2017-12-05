namespace CameraBazaar.Data.Common
{
    public static class PropertyErrorMessageConstants
    {
        public const string ModelErrorMessage = "{0} must be less than {1} symbols long.";

        public const string PriceErrorMessage = "{0} must be positive number.";

        public const string QuantityErrorMessage = "{0} must be between {1} and {2}.";

        public const string MinShutterSpeedErrorMessage = "{0} must be between {1} and {2}.";

        public const string MaxShutterSpeedErrorMessage = "{0} must be between {1} and {2}.";

        public const string MaxISOErrorMessage = "{0} must be between {1} and {2}.";

        public const string VideoResolutionErrorMessage = "{0} must be less than {1} symbols long.";

        public const string DescriptionErrorMessage = "{0} must be less than {1} symbols long.";

        public const string ImageUrlErrorMessage = "{0} must be between {2} and {1} symbols long.";

        public const string EmailErrorMessage = "{0} must be between {2} and {1} symbols long.";

        public const string PhoneErrorMessage = "Phone number must start with a '+' sign and contain between 10 and 12 digits.";

        public const string ModelContentErrorMessage = "{0} can contain only uppercase letters, digits and dash ('-').";

        public const string MaxISOContentErrorMessage = "Max ISO must be dividable by 100.";

        public const string ImageUrlContentErrorMessage = "Image URL must start with http:// or https://.";
    }
}