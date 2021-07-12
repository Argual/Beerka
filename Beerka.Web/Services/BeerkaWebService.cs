using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Beerka.Persistence;
using Beerka.Web.Models;

namespace Beerka.Web.Services
{
    public class BeerkaWebService : Persistence.Services.BeerkaService
    {
        public BeerkaWebService(BeerkaContext context) : base(context)
        {

        }

        public ShoppingCartItemError ValidateShoppingCartItem(ShoppingCart cart, Product product, Product.Packaging.PackagingType packaging, int amount)
        {
            if (packaging.DbValue != Product.Packaging.Unit.DbValue && product.PackagingType.DbValue != packaging.DbValue)
            {
                return ShoppingCartItemError.InvalidPackaging;
            }

            int amountInUnits = packaging.UnitCount * amount;
            if (amountInUnits < 0 || amountInUnits > product.Stock-GetProductReservedAmount(product))
            {
                return ShoppingCartItemError.InvalidAmount;
            }

            return ShoppingCartItemError.None;
        }
    }
}
