using Beerka.Desktop.Model;
using Beerka.Persistence.DTO;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Beerka.Desktop.ViewModel
{
    public class OrderViewModel : ViewModelBase
    {
        public OrderViewModel(List<ProductDTO> products, OrderDTO orderDTO)
        {
            ID = orderDTO.ID;
            CustomerName = orderDTO.CustomerName;
            CustomerAddress = orderDTO.CustomerAddress;
            CustomerPhone = orderDTO.CustomerPhone;
            CustomerEmail = orderDTO.CustomerEmail;
            IsDelivered = orderDTO.IsDelivered;

            _amounts = new List<int>(orderDTO.Amounts);
            _productIDs = new List<int>(orderDTO.ProductIDs);

            var productOrders = new List<string>();
            for (int i = 0; i < orderDTO.ProductIDs.Count; i++)
            {
                var productID = orderDTO.ProductIDs[i];
                var productDTO = products.Single(p => p.ID == productID);
                productOrders.Add(productDTO.Name + " (" + orderDTO.Amounts[i] + " Units)");
            }
            ProductOrders = "";
            for (int i = 0; i < productOrders.Count; i++)
            {
                if (i>0)
                {
                    ProductOrders += ", ";
                }
                ProductOrders += productOrders[i];
            }
        }

        [Key]
        public int ID { get; private set; }

        [Required]
        public string CustomerName { get; private set; }

        [Required]
        public string CustomerAddress { get; private set; }

        [Required]
        public string CustomerPhone { get; private set; }

        [Required]
        public string CustomerEmail { get; private set; }

        [Required]
        public bool IsDelivered { get; set; }

        private List<int> _productIDs;
        private List<int> _amounts;

        public string ProductOrders { get; private set; }

        public static explicit operator OrderDTO(OrderViewModel orderViewModel)
        {
            return new OrderDTO
            {
                ID = orderViewModel.ID,
                Amounts = new List<int>(orderViewModel._amounts),
                ProductIDs = new List<int>(orderViewModel._productIDs),
                CustomerAddress = orderViewModel.CustomerAddress,
                CustomerEmail = orderViewModel.CustomerEmail,
                CustomerName = orderViewModel.CustomerName,
                CustomerPhone = orderViewModel.CustomerPhone,
                IsDelivered = orderViewModel.IsDelivered
            };
        }

    }
}
