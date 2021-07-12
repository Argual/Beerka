using Beerka.Persistence;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Beerka.Web.Models
{
    public class ShoppingCartItemViewModel
    {

        public Product Product { get; set; }

        [Required(ErrorMessage = "Specifying the packaging is required!")]
        public int PackagingTypeID { get; set; }

        [Required(ErrorMessage = "Specifying the amount of products to buy is required!")]
        public int Amount { get; set; }
    }
}
