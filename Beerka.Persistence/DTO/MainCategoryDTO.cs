using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Beerka.Persistence.DTO
{
    public class MainCategoryDTO
    {
        [Key]
        public int ID { get; set; }

        [Required]
        public string Name { get; set; }

        public static explicit operator MainCategory(MainCategoryDTO mainCategoryDTO)
        {
            if (mainCategoryDTO == null)
            {
                throw new ArgumentNullException(nameof(mainCategoryDTO), "'" + nameof(mainCategoryDTO) + "' must not be null!");
            }

            return new MainCategory {
                ID = mainCategoryDTO.ID,
                Name = mainCategoryDTO.Name
            };
        }

        public static explicit operator MainCategoryDTO(MainCategory mainCategory)
        {
            if (mainCategory == null)
            {
                throw new ArgumentNullException(nameof(mainCategory), "'" + nameof(mainCategory) + "' must not be null!");
            }

            return new MainCategoryDTO {
                ID = mainCategory.ID,
                Name = mainCategory.Name
            };
        }
    }
}
