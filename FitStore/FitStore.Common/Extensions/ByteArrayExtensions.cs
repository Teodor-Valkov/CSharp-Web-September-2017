namespace FitStore.Common.Extensions
{
    using System;

    public static class ByteArrayExtensions
    {
        public static string FromByteArrayToString(this byte[] byteArray)
        {
            string byteArrayAsBase64 = Convert.ToBase64String(byteArray);
            string byteArrayAsString = string.Format("data:image/gif;base64,{0}", byteArrayAsBase64);

            return byteArrayAsString;
        }
    }
}