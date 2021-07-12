using Beerka.Persistence.DTO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Text;
using System.Windows.Controls;
using System.Windows.Data;

namespace Beerka.Desktop.ViewModel
{
    public class ProductViewModel : ViewModelBase, IEditableObject, IDataErrorInfo
    {
        #region Data Fields
        
        private string _name;
        private string _manufacturer;
        private string _modelNumber;
        private string _description;
        private int _priceNet;
        private int _stock;
        private string _packagingTypeString;

        public int ID { get; set; }
        public int SubCategoryID { get; set; }

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged();
            }
        }
        public string Manufacturer
        {
            get => _manufacturer;
            set
            {
                _manufacturer = value;
                OnPropertyChanged();
            }
        }
        public string ModelNumber
        {
            get => _modelNumber;
            set
            {
                _modelNumber = value;
                OnPropertyChanged();
            }
        }
        public string Description
        {
            get => _description;
            set
            {
                _description = value;
                OnPropertyChanged();
            }
        }

        public int PriceNet
        {
            get => _priceNet;
            set
            {
                _priceNet = value;
                OnPropertyChanged();
            }
        }
        public int Stock
        {
            get => _stock;
            set
            {
                _stock = value;
                OnPropertyChanged();
            }
        }
        public Persistence.Product.Packaging.PackagingType PackagingType
        {
            get => Persistence.Product.Packaging.GetPackagingTypeFromDbValue(_packagingTypeString);
            set
            {
                _packagingTypeString = value.DbValue;
            }
        }

        #endregion

        #region Display properties

        [DisplayName("Price (Net)")]
        public string PriceNetDisplay
        {
            get
            {
                return $"{PriceNet} Ft";
            }
        }

        [DisplayName("Price (Gross)")]
        public string PriceGrossDisplay
        {
            get
            {
                return $"{(int)Math.Ceiling(PriceNet * 1.27)} Ft";
            }
        }

        [DisplayName("Available Packagings")]
        public string PackagingDisplay
        {
            get
            {
                string packagingDisplay = "Unit";
                if (PackagingType.DbValue != Persistence.Product.Packaging.Unit.DbValue)
                {
                    packagingDisplay += ", " + PackagingType.DisplayName;
                }
                return packagingDisplay;
            }
        }

        #endregion

        public event EventHandler EditEnded;

        private ProductViewModel _backup;

        public String this[string columnName]
        {
            get => Validate(columnName);
        }

        public string Error
        {
            get {
                List<string> propertiesToValidate = new List<string>()
                {
                    nameof(Name),
                    nameof(ModelNumber),
                    nameof(Manufacturer),
                    nameof(Stock),
                    nameof(PriceNet),
                    nameof(Description)
                };
                foreach (var property in propertiesToValidate)
                {
                    string error = Validate(property);
                    if (error!=String.Empty)
                    {
                        return error;
                    }
                }
                return string.Empty;
            }
        }

        public string Validate(string propertyName)
        {
            switch (propertyName)
            {
                case (nameof(Name)):
                    if (String.IsNullOrEmpty(Name))
                    {
                        return "Name cannot be empty.";
                    }
                    break;
                case (nameof(ModelNumber)):
                    if (String.IsNullOrEmpty(ModelNumber))
                    {
                        return "Model Number cannot be empty.";
                    }
                    break;
                case (nameof(Manufacturer)):
                    if (String.IsNullOrEmpty(Manufacturer))
                    {
                        return "Manufacturer cannot be empty.";
                    }
                    break;
                case (nameof(Stock)):
                    if (Stock < 0)
                    {
                        return "Stock cannot be negative.";
                    }
                    break;
                case (nameof(PriceNet)):
                    if (PriceNet < 0)
                    {
                        return "Price cannot be negative.";
                    }
                    break;
                case (nameof(Description)):
                    if (String.IsNullOrEmpty(Description))
                    {
                        return "Description cannot be empty.";
                    }
                    break;
                default:
                    break;
            }
            return String.Empty;
        }

        public Boolean IsDirty { get; private set; } = false;

        public void BeginEdit()
        {
            if (!IsDirty)
            {
                _backup = (ProductViewModel)this.MemberwiseClone();
                IsDirty = true;
            }
        }

        public void CancelEdit()
        {
            if (IsDirty)
            {
                ID = _backup.ID;
                SubCategoryID = _backup.SubCategoryID;
                Name = _backup.Name;
                ModelNumber = _backup.ModelNumber;
                Manufacturer = _backup.Manufacturer;
                Description = _backup.Description;
                PackagingType = _backup.PackagingType;
                PriceNet = _backup.PriceNet;
                Stock = _backup.Stock;

                IsDirty = false;
                _backup = null;
            }
        }

        public void EndEdit()
        {
            if (IsDirty)
            {
                IsDirty = false;
                _backup = null;
                EditEnded?.Invoke(this, EventArgs.Empty);
            }
        }

        #region Conversion

        public static explicit operator ProductViewModel(ProductDTO productDTO)
        {
            ProductViewModel productViewModel = new ProductViewModel
            {
                ID = productDTO.ID,
                SubCategoryID = productDTO.SubCategoryID,
                Name = productDTO.Name,
                Manufacturer = productDTO.Manufacturer,
                ModelNumber = productDTO.ModelNumber,
                Description = productDTO.Description,
                PackagingType = Persistence.Product.Packaging.GetPackagingTypeFromDbValue(productDTO.PackagingTypeString),
                PriceNet = productDTO.PriceNet,
                Stock = productDTO.Stock
            };
            return productViewModel;
        }

        public static explicit operator ProductDTO(ProductViewModel productViewModel)
        {
            ProductDTO productDTO = new ProductDTO
            {
                ID = productViewModel.ID,
                SubCategoryID = productViewModel.SubCategoryID,
                Name = productViewModel.Name,
                Manufacturer = productViewModel.Manufacturer,
                ModelNumber = productViewModel.ModelNumber,
                Description = productViewModel.Description,
                PackagingTypeString = productViewModel.PackagingType.DbValue,
                PriceNet = productViewModel.PriceNet,
                Stock = productViewModel.Stock
            };
            return productDTO;
        }

        #endregion

    }
}
