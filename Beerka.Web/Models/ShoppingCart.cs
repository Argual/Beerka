using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Beerka.Web.Models
{
    public partial class ShoppingCart
    {

        public List<ShoppingCartItem> Items { get; set; }

        /// <summary>
        /// The gross price of all items in the shopping cart.
        /// </summary>
        public int PriceGross
        {
            get
            {
                return Items.Select(i => i.PriceGross).Sum();
            }
        }

        /// <summary>
        /// The net price of all items in the shopping cart.
        /// </summary>
        public int PriceNet
        {
            get
            {
                return Items.Select(i => i.PriceNet).Sum();
            }
        }

        public ShoppingCart()
        {
            Items = new List<ShoppingCartItem>();
        }

    }
}
