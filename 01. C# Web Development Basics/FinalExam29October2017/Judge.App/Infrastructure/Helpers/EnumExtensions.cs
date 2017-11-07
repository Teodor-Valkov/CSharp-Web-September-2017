namespace Judge.App.Infrastructure.Helpers
{
    using Data.Models.Enums;
    using System;

    public static class EnumExtensions
    {
        public static string ToFriendlyName(this BuildResultType type)
        {
            switch (type)
            {
                case BuildResultType.BuildFailed:
                    return "Build Failed";

                case BuildResultType.BuildSuccess:
                    return "Build Success";

                default:
                    throw new InvalidOperationException($"Invalid position type {type}.");
            }
        }

        public static string SetTypeColor(this BuildResultType type)
        {
            switch (type)
            {
                case BuildResultType.BuildFailed:
                    return "danger";

                case BuildResultType.BuildSuccess:
                    return "success";

                default:
                    throw new InvalidOperationException($"Invalid log type {type}.");
            }
        }
    }
}