namespace CameraBazaar.Data.Common
{
    public static class PropertyValidationConstants
    {
        public const int ModelMaxLength = 100;

        public const double PriceMinValue = 0;

        public const double PriceMaxValue = double.MaxValue;

        public const int QuantityMinValue = 0;

        public const int QuantityMaxValue = 100;

        public const int MinShutterSpeedMinValue = 0;

        public const int MinShutterSpeedMaxValue = 30;

        public const int MaxShutterSpeedMinValue = 2000;

        public const int MaxShutterSpeedMaxValue = 8000;

        public const int MaxISOMinValue = 200;

        public const int MaxISOMaxValue = 409600;

        public const int VideoResolutionMaxLength = 15;

        public const int DescriptionMaxLength = 6000;

        public const int ImageUrlMinLength = 10;

        public const int ImageUrlMaxLength = 2000;

        public const int EmailMinLength = 5;

        public const int EmailMaxLength = 100;
    }
}