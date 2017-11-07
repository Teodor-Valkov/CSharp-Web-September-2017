namespace GameStore.GameStoreApplication.Utilities
{
    using System.ComponentModel.DataAnnotations;

    public class ImageAttribute : ValidationAttribute
    {
        public ImageAttribute()
        {
            this.ErrorMessage = "Image URL should start with 'http://' or 'https://'! Otherwise it should be empty!";
        }

        public override bool IsValid(object value)
        {
            string image = value as string;

            if (image == null)
            {
                return true;
            }

            return image.StartsWith("http://") || image.StartsWith("https://");
        }
    }
}