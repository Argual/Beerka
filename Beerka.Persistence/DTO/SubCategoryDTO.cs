using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Beerka.Persistence.DTO
{
    public class SubCategoryDTO
    {
        [Key]
        public int ID { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public int MainCategoryID { get; set; }

        public static explicit operator SubCategory(SubCategoryDTO subCategoryDTO)
        {
            if (subCategoryDTO == null)
            {
                throw new ArgumentNullException(nameof(subCategoryDTO), "'" + nameof(subCategoryDTO) + "' must not be null!");
            }

            return new SubCategory {
                ID = subCategoryDTO.ID,
                Name = subCategoryDTO.Name,
                MainCategoryID = subCategoryDTO.MainCategoryID
            };
        }

        public static explicit operator SubCategoryDTO(SubCategory subCategory)
        {
            if (subCategory == null)
            {
                throw new ArgumentNullException(nameof(subCategory), "'" + nameof(subCategory) + "' must not be null!");
            }

            return new SubCategoryDTO {
                ID = subCategory.ID,
                MainCategoryID = subCategory.MainCategoryID,
                Name = subCategory.Name,
            };
        }
    }
}
