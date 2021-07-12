using Beerka.Persistence.DTO;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Beerka.Desktop.Model
{
    public class BeerkaAPIService
    {
        private readonly HttpClient _client;

        public BeerkaAPIService(string baseAddress)
        {
            _client = new HttpClient()
            {
                BaseAddress = new Uri(baseAddress)
            };
        }

        #region Authentication

        public async Task<bool> LoginAsync(string name, string password)
        {
            LoginDTO user = new LoginDTO
            {
                UserName = name,
                Password = password
            };

            HttpResponseMessage response = await _client.PostAsJsonAsync("api/Account/Login", user);

            if (response.IsSuccessStatusCode)
            {
                return true;
            }

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                return false;
            }

            throw new NetworkException("Service returned response: " + response.StatusCode);
        }

        public async Task LogoutAsync()
        {
            HttpResponseMessage response = await _client.PostAsync("api/Account/Logout", null);

            if (response.IsSuccessStatusCode)
            {
                return;
            }

            throw new NetworkException("Service returned response: " + response.StatusCode);
        }

        #endregion

        #region MainCategories

        public async Task<IEnumerable<MainCategoryDTO>> LoadMainCategoriesAsync()
        {
            HttpResponseMessage response = await _client.GetAsync("api/MainCategories/");

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsAsync<IEnumerable<MainCategoryDTO>>();
            }

            throw new NetworkException("Service returned response: " + response.StatusCode);
        }

        #endregion

        #region SubCategories

        public async Task<IEnumerable<SubCategoryDTO>> LoadSubCategoriesAsync(int mainCategoryID)
        {
            HttpResponseMessage response = await _client.GetAsync($"api/SubCategories/{mainCategoryID}");

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsAsync<IEnumerable<SubCategoryDTO>>();
            }

            throw new NetworkException("Service returned response: " + response.StatusCode);
        }

        #endregion

        #region Products

        public async Task<ProductDTO> LoadProductAsync(int productID)
        {
            HttpResponseMessage response = await _client.GetAsync($"api/Products/{productID}");

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsAsync<ProductDTO>();
            }

            throw new NetworkException("Service returned response: " + response.StatusCode);
        }

        public async Task<IEnumerable<ProductDTO>> LoadProductsAsync(int mainCategoryID, int subCategoryID)
        {
            HttpResponseMessage response = await _client.GetAsync($"api/Products/{mainCategoryID}/{subCategoryID}");

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsAsync<IEnumerable<ProductDTO>>();
            }

            throw new NetworkException("Service returned response: " + response.StatusCode);
        }

        public async Task CreateProductAsync(ProductDTO productDTO)
        {
            HttpResponseMessage response = await _client.PostAsJsonAsync("api/Products/", productDTO);

            if (!response.IsSuccessStatusCode)
            {
                throw new NetworkException("Service returned response: " + response.StatusCode);
            }
        }

        public async Task UpdateProductAsync(ProductDTO productDTO)
        {
            HttpResponseMessage response = await _client.PutAsJsonAsync($"api/Products/{productDTO.ID}", productDTO);

            if (!response.IsSuccessStatusCode)
            {
                throw new NetworkException("Service returned response: " + response.StatusCode);
            }
        }

        #endregion

        #region Orders

        public async Task<IEnumerable<OrderDTO>> LoadOrdersAsync()
        {
            HttpResponseMessage response = await _client.GetAsync("api/Orders/");

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsAsync<IEnumerable<OrderDTO>>();
            }

            throw new NetworkException("Service returned response: " + response.StatusCode);
        }

        public async Task UpdateOrderAsync(OrderDTO orderDTO)
        {
            HttpResponseMessage response = await _client.PutAsJsonAsync($"api/Orders/{orderDTO.ID}", orderDTO);

            if (!response.IsSuccessStatusCode)
            {
                throw new NetworkException("Service returned response: " + response.StatusCode);
            }
        }

        #endregion
    }
}
