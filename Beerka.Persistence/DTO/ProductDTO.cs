using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Beerka.Persistence.DTO
{
    public class ProductDTO
    {
        [Key]
        public int ID { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Manufacturer { get; set; }

        [Required]
        public string ModelNumber { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public int SubCategoryID { get; set; }

        [Required]
        public int PriceNet { get; set; }

        [Required]
        public int Stock { get; set; }

        public string PackagingTypeString { get; set; }

        #region Conversion

        /// <summary>
        /// Converts given product DTO to product.
        /// </summary>
        public static explicit operator Product(ProductDTO productDTO)
        {
            if (productDTO == null)
            {
                throw new ArgumentNullException(nameof(productDTO),"'" + nameof(productDTO) + "' must not be null!");
            }

            Product product = new Product
            {
                PackagingType = Product.Packaging.GetPackagingTypeFromDbValue(productDTO.PackagingTypeString),
                PriceNet = productDTO.PriceNet,
                Description = productDTO.Description,
                ID = productDTO.ID,
                Manufacturer = productDTO.Manufacturer,
                ModelNumber = productDTO.ModelNumber,
                Name = productDTO.Name,
                Stock = productDTO.Stock,
                SubCategoryID = productDTO.SubCategoryID
            };

            return product;
        }

        /// <summary>
        /// Converts given product to product DTO.
        /// </summary>
        public static explicit operator ProductDTO(Product product)
        {
            if (product == null)
            {
                throw new ArgumentNullException(nameof(product), "'" + nameof(product) + "' must not be null!");
            }

            ProductDTO productDTO = new ProductDTO
            {
                PackagingTypeString = product.PackagingType.DbValue,
                PriceNet = product.PriceNet,
                Description = product.Description,
                ID = product.ID,
                Manufacturer = product.Manufacturer,
                ModelNumber = product.ModelNumber,
                Name = product.Name,
                Stock = product.Stock,
                SubCategoryID = product.SubCategoryID
            };

            return productDTO;
        }

        #endregion

    }
}
