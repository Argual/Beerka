using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace Beerka.Persistence
{
    public partial class Product
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

        [ForeignKey("SubCategoryID")]
        public virtual SubCategory SubCategory {get; set;}

        [Required]
        public int PriceNet { get; set; }

        [Required]
        public int Stock { get; set; }


        protected string _packagingTypeString { get; set; }

        [NotMapped]
        public Packaging.PackagingType PackagingType
        {
            get { return Packaging.GetPackagingTypeFromDbValue(_packagingTypeString); }
            set
            {
                _packagingTypeString = value.DbValue;
            }
        }

    }
}
