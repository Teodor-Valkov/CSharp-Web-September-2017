namespace _02.SocialNetwork.Client.Utilities
{
    public static class TagTransformer
    {
        private const int MaxLength = 20;

        public static string Transform(string tag)
        {
            string transformedTag = tag;

            transformedTag = tag.Replace(" ", string.Empty);

            if (!transformedTag.StartsWith('#'))
            {
                transformedTag = "#" + transformedTag;
            }

            if (transformedTag.Length > MaxLength)
            {
                transformedTag = tag.Substring(0, MaxLength);
            }

            return transformedTag;
        }
    }
}