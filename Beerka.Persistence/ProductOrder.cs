using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Beerka.Persistence
{
    public class ProductOrder
    {
        [Required]
        public int ProductID { get; set; }

        public virtual Product Product { get; set; }

        [Required]
        public int OrderID { get; set; }

        public virtual Order Order { get; set; }

        [Required]
        public int Amount { get; set; }
    }
}
