using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Beerka.Web.Models
{
    public class FinalizationViewModel
    {
        private const string extraAllowedLetters = "áÁéÉíÍóÓöÖőŐúÚüÜűŰ";
        private const string regexStringLetters = "[A-Za-z" + extraAllowedLetters + "]";
        private const string regexStringName = "^("+ regexStringLetters + "+)([' ']"+regexStringLetters+"+)*$";

        private string Capitalize(string str)
        {
            var parts = str.Split(' ');
            string s = "";
            foreach (var p in parts)
            {
                if (p.Length>0)
                {
                    s += p[0].ToString().ToUpper();
                }
                if (p.Length>1)
                {
                    s += p[1..].ToLower();
                }
                if (p.Length > 0)
                {
                    s += " ";
                }
            }
            s = s.TrimEnd();
            return s;
        }

        [Required(ErrorMessage ="Providing a name is required!")]
        [RegularExpression(regexStringName, ErrorMessage = "A valid name must be provided!")]
        public string NameRawInput { get; set; }

        public string Name
        {
            get
            {
                return string.IsNullOrEmpty(NameRawInput) ? "" :Capitalize(NameRawInput);
            }
        }

        [Required(ErrorMessage = "Providing a zip code is required!")]
        [RegularExpression("^[0-9]{4}$", ErrorMessage ="Zip code length or value is invalid.")]
        public string ZipCode { get; set; }

        [Required(ErrorMessage = "Providing a city name is required!")]
        [RegularExpression(regexStringName, ErrorMessage = "City length or value is invalid.")]
        public string CityRawInput { get; set; }

        public string City
        {
            get
            {
                return string.IsNullOrEmpty(CityRawInput) ? "" : Capitalize(CityRawInput);
            }
        }

        [Required(ErrorMessage = "Providing a public space name is required!")]
        [RegularExpression(regexStringName, ErrorMessage = "Public space name length or value is invalid.")]
        public string PublicSpaceNameRawInput { get; set; }

        public string PublicSpaceName
        {
            get
            {
                return string.IsNullOrEmpty(PublicSpaceNameRawInput) ? "" : Capitalize(PublicSpaceNameRawInput);
            }
        }

        [Required(ErrorMessage ="Providing a house number is required!")]
        [RegularExpression("^[1-9]+[0-9]*([/][A-Za-z])?$", ErrorMessage ="House number format is invalid! Acceptable examples: '3','3/A'.")]
        public string HouseNumberRawInput { get; set; }

        public string HouseNumber
        {
            get
            {
                return string.IsNullOrEmpty(HouseNumberRawInput) ? "" : HouseNumberRawInput.ToUpper();
            }
        }
        

        [Required(ErrorMessage = "Providing an e-mail address is required!")]
        [RegularExpression("^[A-Za-z]+([A-Za-z0-9.]*[A-Za-z0-9])?@([A-Za-z0-9.]*[A-Za-z0-9])[.][A-Za-z]+$",ErrorMessage ="Must be a valid e-mail address!")]
        public string EmailRawInput { get; set; }

        public string Email
        {
            get
            {
                return string.IsNullOrEmpty(EmailRawInput) ? "" : EmailRawInput.ToLower();
            }
        }

        [Required(ErrorMessage = "Providing a phone number is required!")]
        [RegularExpression("^[+]?[0-9]+[0-9 -]*[0-9]+$",ErrorMessage ="Phone number must be valid!")]
        public string PhoneRawInput { get; set; }

        public string Phone
        {
            get
            {
                if (string.IsNullOrEmpty(PhoneRawInput))
                {
                    return "";
                }
                string phone = "";
                foreach (char c in PhoneRawInput)
                {
                    if ("0123456789".Contains(c))
                    {
                        phone += c;
                    }
                }
                return phone;
            }
        }

        public string Address
        { get
            {
                return ZipCode+" "+City+", "+PublicSpaceName+", "+HouseNumber;
            }
        }
    }
}
