using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Beerka.Persistence
{
    public class Order
    {

        [Key]
        public int ID { get; set; }

        [Required]
        public string CustomerName { get; set; }

        [Required]
        public string CustomerAddress { get; set; }

        [Required]
        public string CustomerPhone { get; set; }

        [Required]
        public string CustomerEmail { get; set; }

        public virtual ICollection<ProductOrder> ProductOrders { get; set; }

        [Required]
        public bool IsDelivered { get; set; }

    }
}
