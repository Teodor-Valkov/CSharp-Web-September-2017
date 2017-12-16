namespace FitStore.Web.Infrastructure.Extensions
{
    using Microsoft.AspNetCore.Http;
    using Newtonsoft.Json;
    using System;

    using static Common.CommonConstants;

    public static class SessionExtensions
    {
        //public static string GetShoppingCartId(this ISession session)
        //{
        //    string shoppingCartId = session.GetString(UserSessionShoppingCartKey);

        //    if (shoppingCartId == null)
        //    {
        //        shoppingCartId = Guid.NewGuid().ToString();
        //        session.SetString(UserSessionShoppingCartKey, shoppingCartId);
        //    }

        //    return shoppingCartId;
        //}

        public static T GetShoppingCart<T>(this ISession session, string key)
            where T : class
        {
            string value = session.GetString(key);

            return value == null
                ? Activator.CreateInstance<T>()
                : JsonConvert.DeserializeObject<T>(value);
        }

        public static void SetShoppingCart<T>(this ISession session, string key, T value)
            where T : class
        {
            session.SetString(key, JsonConvert.SerializeObject(value));
        }
    }
}