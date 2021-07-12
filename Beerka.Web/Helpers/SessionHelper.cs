using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;
using Beerka.Web.Models;
using Beerka.Web.Services;
using Beerka.Persistence;

namespace Beerka.Web.Helpers
{
    public static class SessionHelper
    {
        /// <summary>
        /// Sets the given key and serializable object's serialized value in the given session.
        /// </summary>
        /// <typeparam name="T">The type of the object to serialize and store.</typeparam>
        /// <param name="session">The session to set key and value in.</param>
        /// <param name="key">The key for the object.</param>
        /// <param name="value">The serializable object to serialize and store in the session.</param>
        public static void SetT<T>(ISession session, string key, T value)
        {
            string jsonStr = JsonSerializer.Serialize<T>(value);
            session.SetString(key, jsonStr);
        }

        /// <summary>
        /// Retrieves the object belonging to the given key from the given session.
        /// </summary>
        /// <typeparam name="T">The type of the object to serialize and store.</typeparam>
        /// <param name="session">The session to retrieve the object from.</param>
        /// <param name="key">The key for the object.</param>
        /// <returns>Object belonging to the given key in the given session. If it wasn't found, a default object will be returned instead.</returns>
        public static T GetT<T>(ISession session, string key)
        {
            string jsonStr = session.GetString(key);
            if (string.IsNullOrEmpty(jsonStr))
            {
                return default(T);
            }
            return JsonSerializer.Deserialize<T>(jsonStr);
        }

        private static string shoppingCartItemToString(ShoppingCart.ShoppingCartItem item)
        {
            string packagingTypeStr = string.IsNullOrEmpty(item.PackagingType.DbValue) ? "" : item.PackagingType.DbValue;
            return item.Product.ID.ToString() + "," + packagingTypeStr + "," + item.PackAmount.ToString();
        }

        private static ShoppingCart.ShoppingCartItem shoppingCartItemFromString(string str, BeerkaWebService service)
        {
            string[] parts = str.Split(',');
            string dbValue = string.IsNullOrEmpty(parts[1]) ? null : parts[1];
            var item = new ShoppingCart.ShoppingCartItem
            {
                Product=service.GetProduct(Int32.Parse(parts[0])),
                PackagingType=Product.Packaging.GetPackagingTypeFromDbValue(dbValue),
                PackAmount=Int32.Parse(parts[2])
            };
            return item;
        }

        public static void SetShoppingCart(ISession session, string key, ShoppingCart cart)
        {
            if (cart.Items==null)
            {
                cart.Items = new List<ShoppingCart.ShoppingCartItem>();
            }
            List<string> shoppingCartItemStrings = cart.Items.Select(i => shoppingCartItemToString(i)).ToList();
            SetT(session, key, shoppingCartItemStrings);
        }

        public static ShoppingCart GetShoppingCart(ISession session, string key, BeerkaWebService service)
        {
            List<string> shoppingCartItemStrings = GetT<List<string>>(session, key);
            if (shoppingCartItemStrings==null)
            {
                shoppingCartItemStrings = new List<string>();
            }
            ShoppingCart cart = new ShoppingCart
            {
                Items = shoppingCartItemStrings.Select(s=>shoppingCartItemFromString(s,service)).ToList()
            };
            return cart;
        }
    }
}
