using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Beerka.Persistence
{
    public partial class Product
    {
        public static partial class Packaging
        {
            public struct PackagingType
            {
                /// <summary>
                /// The value that represents this packaging type in the database.
                /// </summary>
                public string DbValue { get; set; }

                /// <summary>
                /// The display name for this packaging type.
                /// </summary>
                public string DisplayName { get; set; }

                /// <summary>
                /// The display name for this packaging type in plural.
                /// </summary>
                public string DisplayNamePlural { get; set; }

                /// <summary>
                /// The amount of units this packaging type represents.
                /// </summary>
                public int UnitCount { get; set; }
            }
        }
    }
}
