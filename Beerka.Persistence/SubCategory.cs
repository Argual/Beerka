using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Beerka.Persistence
{
    public class SubCategory
    {

        [Key]
        public int ID { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public int MainCategoryID { get; set; }

        public virtual MainCategory MainCategory { get; set; }

    }
}
