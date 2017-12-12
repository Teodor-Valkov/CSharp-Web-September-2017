namespace FitStore.Web.Infrastructure.Extensions
{
    using Microsoft.AspNetCore.Http;
    using Newtonsoft.Json;
    using Services.Models;

    public static class SessionExtensions
    {
        public static void SetShoppingCart(this ISession session, string shoppingCartKey, ShoppingCart shoppingCart)
        {
            session.SetString(shoppingCartKey, JsonConvert.SerializeObject(shoppingCart));
        }

        public static ShoppingCart GetShoppingCart(this ISession session, string shoppingCartKey)
        {
            string shoppingCartAsString = session.GetString(shoppingCartKey);

            return shoppingCartAsString == null
                ? new ShoppingCart()
                : JsonConvert.DeserializeObject<ShoppingCart>(shoppingCartAsString);
        }

        //public static void Set<T>(this ISession session, string key, T value)
        //{
        //    session.SetString(key, JsonConvert.SerializeObject(value));
        //}

        //public static T Get<T>(this ISession session, string key)
        //{
        //    string value = session.GetString(key);

        //    return value == null
        //        ? default(T)
        //        : JsonConvert.DeserializeObject<T>(value);
        //}
    }
}