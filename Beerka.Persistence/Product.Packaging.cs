using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Beerka.Persistence
{
    public partial class Product
    {


        /// <summary>
        /// Packaging types products can be sold in.
        /// </summary>
        public static partial class Packaging
        {
            

            /// <summary>
            /// Possible packaging type identifiers and their size respectively.
            /// </summary>
            public static readonly PackagingType[] PackagingTypes = {
                new PackagingType
                {
                    DbValue=null,
                    DisplayName="Unit",
                    DisplayNamePlural="Units",
                    UnitCount=1
                },
                new PackagingType
                {
                    DbValue="WRAP",
                    DisplayName="Shrinkwrap",
                    DisplayNamePlural="Shrinkwraps",
                    UnitCount=6
                },
                new PackagingType
                {
                    DbValue="CRATE",
                    DisplayName="Crate",
                    DisplayNamePlural="Crates",
                    UnitCount=12
                },
                new PackagingType
                {
                    DbValue="TRAY",
                    DisplayName="Tray",
                    DisplayNamePlural="Trays",
                    UnitCount=24
                },
            };

            public static PackagingType Unit { get { return PackagingTypes[0]; } }
            public static PackagingType Shrinkwrap { get { return PackagingTypes[1]; } }
            public static PackagingType Crate { get { return PackagingTypes[2]; } }
            public static PackagingType Tray { get { return PackagingTypes[3]; } }

            /// <summary>
            /// Gets the corresponding packaging type of the given database value.
            /// </summary>
            /// <param name="value">The database value.</param>
            /// <returns>Corresponding packaging type of the given database value.</returns>
            public static PackagingType GetPackagingTypeFromDbValue(string value)
            {
                var packagingType = Product.Packaging.Unit;
                if (value == Product.Packaging.Crate.DbValue)
                {
                    packagingType = Product.Packaging.Crate;
                }
                else if (value == Product.Packaging.Shrinkwrap.DbValue)
                {
                    packagingType = Product.Packaging.Shrinkwrap;
                }
                else if (value == Product.Packaging.Tray.DbValue)
                {
                    packagingType = Product.Packaging.Tray;
                }
                return packagingType;
            }
        }

    }
}
