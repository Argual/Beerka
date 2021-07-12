using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Beerka.Persistence
{
    public class Employee : IdentityUser
    {

        [Required]
        public string Name { get; set; }

    }
}
