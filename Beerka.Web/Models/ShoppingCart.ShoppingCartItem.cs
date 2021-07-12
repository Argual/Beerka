using Beerka.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Beerka.Web.Models
{
    public partial class ShoppingCart
    {

        public class ShoppingCartItem
        {
            /// <summary>
            /// The packaging type the product was selected with.
            /// </summary>
            public Product.Packaging.PackagingType PackagingType { get; set; }

            /// <summary>
            /// The selected product.
            /// </summary>
            public Product Product { get; set; }

            /// <summary>
            /// The amount of product put in the cart with the selected packaging.
            /// </summary>
            public int PackAmount { get; set; }

            /// <summary>
            /// The amount of product in units.
            /// </summary>
            public int Amount
            {
                get
                {
                    return PackAmount * PackagingType.UnitCount;
                }
            }

            /// <summary>
            /// The gross price of the products.
            /// </summary>
            public int PriceGross
            {
                get
                {
                    return ((int)Math.Round(Product.PriceNet * 1.27, 0) * Amount);
                }
            }

            /// <summary>
            /// The net price of the products.
            /// </summary>
            public int PriceNet
            {
                get
                {
                    return (Product.PriceNet * Amount);
                }
            }

        }

    }
}
