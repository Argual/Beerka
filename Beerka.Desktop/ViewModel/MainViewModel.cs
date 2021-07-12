using Beerka.Desktop.Model;
using Beerka.Persistence.DTO;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Beerka.Desktop.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        #region Private fields

        private readonly BeerkaAPIService _model;

        private bool _isLoadingMainCategories;
        private bool _isLoadingSubCategories;
        private bool _isLoadingProducts;
        private bool _isLoadingOrders;

        private ObservableCollection<MainCategoryDTO> _mainCategories;
        private ObservableCollection<SubCategoryDTO> _subCategories;

        private MainCategoryDTO _selectedMainCategory;
        private SubCategoryDTO _selectedSubCategory;

        private ObservableCollection<ProductViewModel> _products;
        private ProductViewModel _selectedProduct;
        private int? _selectedPackagingTypeIndex;

        private ObservableCollection<OrderViewModel> _orders;
        private OrderViewModel _selectedOrder;

        private string _userName;

        private string _filterNameText;
        private readonly List<string> _filterComboBoxItems;
        private int _filterComboBoxIndex;

        #endregion

        #region Public properties

        public string UserName {
            get => _userName;
            set
            {
                _userName = value;
                OnPropertyChanged();
            }
        }

        public bool IsLoadingMainCategories
        {
            get => _isLoadingMainCategories;
            set
            {
                _isLoadingMainCategories = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsLoadingProductRelated));
                OnPropertyChanged(nameof(IsNotLoadingProductRelated));
            }
        }

        public bool IsLoadingSubCategories
        {
            get => _isLoadingSubCategories;
            set
            {
                _isLoadingSubCategories = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsLoadingProductRelated));
                OnPropertyChanged(nameof(IsNotLoadingProductRelated));
            }
        }

        public bool IsLoadingProducts
        {
            get => _isLoadingProducts;
            set
            {
                _isLoadingProducts = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsLoadingProductRelated));
                OnPropertyChanged(nameof(IsNotLoadingProductRelated));
            }
        }

        public bool IsLoadingOrders
        {
            get => _isLoadingOrders;
            set
            {
                _isLoadingOrders = value;
                OnPropertyChanged();
            }
        }

        public bool IsLoadingProductRelated
        {
            get => IsLoadingMainCategories || IsLoadingSubCategories || IsLoadingProducts;
        }

        public bool IsNotLoadingProductRelated
        {
            get => !IsLoadingProductRelated;
        }

        public ObservableCollection<MainCategoryDTO> MainCategories
        {
            get => _mainCategories;
            set
            {
                _mainCategories = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<SubCategoryDTO> SubCategories
        {
            get => _subCategories;
            set
            {
                _subCategories = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<ProductViewModel> Products
        {
            get => _products;
            set
            {
                _products = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<OrderViewModel> Orders
        {
            get => _orders;
            set
            {
                _orders = value;
                OnPropertyChanged();
            }
        }

        public MainCategoryDTO SelectedMainCategory
        {
            get => _selectedMainCategory;
            set
            {
                _selectedMainCategory = value;
                OnPropertyChanged();
            }
        }

        public SubCategoryDTO SelectedSubCategory
        {
            get => _selectedSubCategory;
            set
            {
                _selectedSubCategory = value;
                OnPropertyChanged();
            }
        }

        public ProductViewModel SelectedProduct
        {
            get => _selectedProduct;
            set
            {
                _selectedProduct = value;
                OnPropertyChanged();
            }
        }

        public int? SelectedPackagingTypeIndex
        {
            get => _selectedPackagingTypeIndex;
            set
            {
                _selectedPackagingTypeIndex = value;
                if (SelectedProduct!=null)
                {
                    int ind;
                    if (_selectedPackagingTypeIndex==null)
                    {
                        ind = 0;
                    }
                    else
                    {
                        ind = (int)_selectedPackagingTypeIndex;
                    }
                    SelectedProduct.PackagingType = Persistence.Product.Packaging.PackagingTypes[ind];
                }
                OnPropertyChanged();
            }
        }

        public ObservableCollection<string> PackagingTypes
        {
            get
            {
                var packagingTypes = new ObservableCollection<string>();
                foreach (var packagingType in Persistence.Product.Packaging.PackagingTypes)
                {
                    if (packagingType.DbValue==Persistence.Product.Packaging.Unit.DbValue)
                    {
                        packagingTypes.Add("None");
                    }
                    else
                    {
                        packagingTypes.Add(packagingType.DisplayName);
                    }
                }
                return packagingTypes;
            }
        }

        public OrderViewModel SelectedOrder
        {
            get => _selectedOrder;
            set
            {
                _selectedOrder = value;
                OnPropertyChanged();
            }
        }

        public List<string> FilterComboBoxItems
        {
            get => _filterComboBoxItems;
        }

        public int FilterComboBoxIndex
        {
            get => _filterComboBoxIndex;
            set
            {
                _filterComboBoxIndex = value;
                OnPropertyChanged();
            }
        }

        public string FilterNameText
        {
            get => _filterNameText;
            set
            {
                _filterNameText = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Commands

        public DelegateCommand RefreshMainCategoriesCommand { get; private set; }
        public DelegateCommand RefreshSubCategoriesCommand { get; private set; }
        public DelegateCommand RefreshProductsCommand { get; private set; }
        public DelegateCommand LogoutCommand { get; private set; }
        public DelegateCommand AddProductCommand { get; private set; }
        public DelegateCommand EditProductCommand { get; private set; }
        public DelegateCommand RefreshOrdersCommand { get; private set; }
        public DelegateCommand ToggleSelectedOrderDeliveryStatusCommand { get; private set; }

        public DelegateCommand SaveProductEditCommand { get; private set; }
        public DelegateCommand CancelProductEditCommand { get; private set; }

        #endregion

        #region Event Handlers

        public event EventHandler LogoutSucceeded;
        public event EventHandler ProductManipulationStarted;
        public event EventHandler ProductManipulationFinished;

        #endregion

        public MainViewModel(BeerkaAPIService model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            _model = model;
            UserName = "";

            IsLoadingSubCategories = false;
            IsLoadingMainCategories = false;
            IsLoadingProducts = false;
            IsLoadingOrders = false;
            
            MainCategories = new ObservableCollection<MainCategoryDTO>();
            SubCategories = new ObservableCollection<SubCategoryDTO>();
            Products = new ObservableCollection<ProductViewModel>();
            Orders = new ObservableCollection<OrderViewModel>();

            SelectedMainCategory = null;
            SelectedSubCategory = null;
            SelectedProduct = null;
            SelectedOrder = null;

            _filterComboBoxItems = new List<string>()
            {
                "Any", "Delivered", "Undelivered"
            };

            RefreshMainCategoriesCommand = new DelegateCommand(_ => IsNotLoadingProductRelated, _ => LoadMainCategoriesAsync());
            RefreshSubCategoriesCommand = new DelegateCommand(_ => IsNotLoadingProductRelated, _ => LoadSubCategoriesAsync(SelectedMainCategory));
            RefreshProductsCommand = new DelegateCommand(_ => IsNotLoadingProductRelated && SelectedSubCategory!=null, _ => LoadProductsAsync(SelectedSubCategory));
            LogoutCommand = new DelegateCommand(_ => IsNotLoadingProductRelated && !IsLoadingOrders, param => LogoutAsync());
            AddProductCommand = new DelegateCommand(_ => IsNotLoadingProductRelated && SelectedSubCategory != null, _ => AddProduct());
            EditProductCommand = new DelegateCommand(_ => IsNotLoadingProductRelated && SelectedSubCategory != null && SelectedProduct != null, _ => StartProductEdit());
            RefreshOrdersCommand = new DelegateCommand(_ => !IsLoadingOrders, _ => LoadOrdersAsync());
            ToggleSelectedOrderDeliveryStatusCommand = new DelegateCommand(_ => !IsLoadingOrders && SelectedOrder!=null, _ => ToggleOrderDeliveryStatus());

            SaveProductEditCommand = new DelegateCommand(_ => SelectedProduct!=null && SelectedProduct.Error==String.Empty, _ => SaveProductEdit());
            CancelProductEditCommand = new DelegateCommand(_ => CancelProductEdit());
        }

        #region Private methods

        private async void LoadMainCategoriesAsync()
        {
            try
            {
                IsLoadingMainCategories = true;

                Products = new ObservableCollection<ProductViewModel>();
                SubCategories = new ObservableCollection<SubCategoryDTO>();
                SelectedSubCategory = null;
                SelectedMainCategory = null;
                SelectedProduct = null;
                MainCategories = new ObservableCollection<MainCategoryDTO>((await _model.LoadMainCategoriesAsync()).ToList());
                
                IsLoadingMainCategories = false;
            }
            catch (Exception ex) when (ex is NetworkException || ex is HttpRequestException)
            {
                OnMessageApplication($"Unexpected error occured! ({ex.Message})");
            }
        }

        private async void LoadSubCategoriesAsync(MainCategoryDTO mainCategory)
        {
            if (mainCategory == null)
            {
                SubCategories = new ObservableCollection<SubCategoryDTO>();
                return;
            }
            try
            {
                IsLoadingSubCategories = true;

                Products = new ObservableCollection<ProductViewModel>();
                SelectedProduct = null;
                SubCategories = new ObservableCollection<SubCategoryDTO>((await _model.LoadSubCategoriesAsync(mainCategory.ID)).ToList());
                
                IsLoadingSubCategories = false;
            }
            catch (Exception ex) when (ex is NetworkException || ex is HttpRequestException)
            {
                OnMessageApplication($"Unexpected error occured! ({ex.Message})");
            }
        }

        private async void LoadProductsAsync(SubCategoryDTO subCategory)
        {
            if (subCategory == null)
            {
                Products = new ObservableCollection<ProductViewModel>();
                return;
            }
            try
            {
                IsLoadingProducts = true;

                Products = new ObservableCollection<ProductViewModel>(
                    (await _model.LoadProductsAsync(subCategory.MainCategoryID, subCategory.ID))
                    .Select(p=> {
                        var productVM = (ProductViewModel)p;
                        return productVM;
                    }));
                SelectedProduct = null;

                IsLoadingProducts = false;
            }
            catch (Exception ex) when (ex is NetworkException || ex is HttpRequestException)
            {
                OnMessageApplication($"Unexpected error occured! ({ex.Message})");
            }
        }

        private async void LoadOrdersAsync()
        {
            try
            {
                IsLoadingOrders = true;

                SelectedOrder = null;

                var orderDTOs = new List<OrderDTO>((await _model.LoadOrdersAsync()).ToList());

                if (FilterComboBoxIndex==1)
                {
                    // Delivered only
                    orderDTOs.RemoveAll(o => !o.IsDelivered);
                }
                else if (FilterComboBoxIndex == 2)
                {
                    // Undelivered only
                    orderDTOs.RemoveAll(o => o.IsDelivered);
                }

                // Filter name
                if (!String.IsNullOrEmpty(FilterNameText))
                {
                    orderDTOs.RemoveAll(o => !o.CustomerName.Contains(FilterNameText));
                }
                

                var orderedProductIDs = new List<int>();
                foreach (var orderDTO in orderDTOs)
                {
                    foreach (var productID in orderDTO.ProductIDs)
                    {
                        if (!orderedProductIDs.Contains(productID))
                        {
                            orderedProductIDs.Add(productID);
                        }
                    }
                }
                var orderedProducts = new List<ProductDTO>();
                foreach (var orderedProductID in orderedProductIDs)
                {
                    var orderedProduct = await _model.LoadProductAsync(orderedProductID);
                    orderedProducts.Add(orderedProduct);
                }

                Orders = new ObservableCollection<OrderViewModel>();
                foreach (var orderDTO in orderDTOs)
                {
                    Orders.Add(new OrderViewModel(orderedProducts,orderDTO));
                }

                IsLoadingOrders = false;
            }
            catch (Exception ex) when (ex is NetworkException || ex is HttpRequestException)
            {
                OnMessageApplication($"Unexpected error occured! ({ex.Message})");
            }
        }

        private async void ToggleOrderDeliveryStatus()
        {
            if (SelectedOrder==null)
            {
                return;
            }

            string message = "Are you sure you want to mark the selected order as ";
            if (SelectedOrder.IsDelivered)
            {
                message += "undelivered?";
            }
            else
            {
                message += "delivered?";
            }

            MessageBoxResult result = MessageBox.Show(message,
                                          "Confirmation",
                                          MessageBoxButton.YesNo,
                                          MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                SelectedOrder.IsDelivered = !SelectedOrder.IsDelivered;

                try
                {
                    IsLoadingOrders = true;

                    OrderDTO updatedOrder = (OrderDTO)SelectedOrder;
                    await _model.UpdateOrderAsync(updatedOrder);

                    IsLoadingOrders = false;
                }
                catch (Exception ex) when (ex is NetworkException || ex is HttpRequestException)
                {
                    OnMessageApplication($"Unexpected error occured! ({ex.Message})");
                }

                if (!IsLoadingOrders)
                {
                    bool productsNeedRefresh = false;
                    if (SelectedOrder!=null)
                    {
                        foreach (var productID in ((OrderDTO)SelectedOrder).ProductIDs)
                        {
                            if (Products.Select(p=>p.ID).Contains(productID))
                            {
                                productsNeedRefresh = true;
                                break;
                            }
                        }
                    }
                    LoadOrdersAsync();
                    if (productsNeedRefresh && SelectedSubCategory != null && !IsLoadingProductRelated)
                    {
                        LoadProductsAsync(SelectedSubCategory);
                    }
                }
            }

        }

        private void AddProduct()
        {
            var productVM = new ProductViewModel
            {
                Name="",
                Manufacturer="",
                ModelNumber="",
                Description="",
                ID=0,
                SubCategoryID=SelectedSubCategory.ID,
                PackagingType=Persistence.Product.Packaging.Unit,
                PriceNet=0,
                Stock=0
            };
            Products.Add(productVM);
            SelectedProduct = productVM;
            StartProductEdit();
        }

        private void StartProductEdit()
        {
            SelectedProduct.BeginEdit();

            int packagingIndex = 0;
            while (SelectedProduct.PackagingType.DbValue!=Persistence.Product.Packaging.PackagingTypes[packagingIndex].DbValue && packagingIndex<Persistence.Product.Packaging.PackagingTypes.Length)
            {
                packagingIndex++;
            }
            SelectedPackagingTypeIndex = packagingIndex;

            ProductManipulationStarted?.Invoke(this, EventArgs.Empty);
        }

        private void CancelProductEdit()
        {
            if (SelectedProduct is null || !SelectedProduct.IsDirty)
                return;

            if (SelectedProduct.ID == 0)
            {
                Products.Remove(SelectedProduct);
                SelectedProduct = null;
            }
            else
            {
                SelectedProduct.CancelEdit();
            }
            ProductManipulationFinished?.Invoke(this, EventArgs.Empty);
        }

        private async void SaveProductEdit()
        {
            try
            {
                int ind;
                if (SelectedPackagingTypeIndex==null)
                {
                    ind = 0;
                }
                else
                {
                    ind = (int)SelectedPackagingTypeIndex;
                }
                SelectedProduct.PackagingType = Persistence.Product.Packaging.PackagingTypes[ind];

                if (SelectedProduct.ID == 0)
                {
                    var productDTO = (ProductDTO)SelectedProduct;
                    await _model.CreateProductAsync(productDTO);
                    SelectedProduct.ID = productDTO.ID;
                    SelectedProduct.EndEdit();
                }
                else
                {
                    await _model.UpdateProductAsync((ProductDTO)SelectedProduct);
                    SelectedProduct.EndEdit();
                }
            }
            catch (Exception ex) when (ex is NetworkException || ex is HttpRequestException)
            {
                OnMessageApplication($"Unexpected error occured! ({ex.Message})");
            }

            ProductManipulationFinished?.Invoke(this, EventArgs.Empty);

            if (IsNotLoadingProductRelated && SelectedSubCategory != null)
            {
                LoadProductsAsync(SelectedSubCategory);
            }
        }

        #endregion

        #region Authentication

        private async void LogoutAsync()
        {
            try
            {
                await _model.LogoutAsync();
                OnLogoutSuccess();
            }
            catch (Exception ex) when (ex is NetworkException || ex is HttpRequestException)
            {
                OnMessageApplication($"Unexpected error occured! ({ex.Message})");
            }
        }

        private void OnLogoutSuccess()
        {
            LogoutSucceeded?.Invoke(this, EventArgs.Empty);
        }

        #endregion
    }
}
