using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Beerka.Persistence.DTO
{
    public class OrderDTO
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

        [Required]
        public bool IsDelivered { get; set; }

        /// <summary>
        /// Contains the ordered product IDs.
        /// </summary>
        [Required]
        public List<int> ProductIDs { get; set; }

        /// <summary>
        /// Contains the ordered amounts for the ordered products.
        /// </summary>
        [Required]
        public List<int> Amounts { get; set; }


        #region Conversion

        /// <summary>
        /// Converts given order DTO to order.
        /// </summary>
        public static explicit operator Order(OrderDTO orderDTO)
        {
            if (orderDTO == null)
            {
                throw new ArgumentNullException(nameof(orderDTO), "'" + nameof(orderDTO) + "' must not be null!");
            }

            if (orderDTO.ProductIDs.Count != orderDTO.Amounts.Count)
            {
                throw new ArgumentException("The count of product IDs and amounts must be equal!", nameof(orderDTO));
            }

            Order order = new Order
            {
                CustomerAddress = orderDTO.CustomerAddress,
                CustomerEmail = orderDTO.CustomerEmail,
                CustomerName = orderDTO.CustomerName,
                CustomerPhone = orderDTO.CustomerPhone,
                ID = orderDTO.ID,
                IsDelivered = orderDTO.IsDelivered,
                ProductOrders = new List<ProductOrder>()
            };

            for (int i = 0; i < orderDTO.ProductIDs.Count; i++)
            {
                order.ProductOrders.Add(new ProductOrder
                {
                    OrderID = order.ID,
                    ProductID = orderDTO.ProductIDs[i],
                    Amount = orderDTO.Amounts[i]
                });
            }

            return order;
        }

        /// <summary>
        /// Converts given order to order DTO.
        /// </summary>
        public static explicit operator OrderDTO(Order order)
        {
            if (order == null)
            {
                throw new ArgumentNullException(nameof(order), "'" + nameof(order) + "' must not be null!");
            }

            OrderDTO orderDTO = new OrderDTO
            {
                CustomerAddress = order.CustomerAddress,
                CustomerEmail = order.CustomerEmail,
                CustomerName = order.CustomerName,
                CustomerPhone = order.CustomerPhone,
                ID = order.ID,
                IsDelivered = order.IsDelivered,
                Amounts = new List<int>(),
                ProductIDs = new List<int>()
            };

            foreach (var po in order.ProductOrders)
            {
                orderDTO.Amounts.Add(po.Amount);
                orderDTO.ProductIDs.Add(po.ProductID);
            }

            return orderDTO;
        }

        #endregion
    }
}
