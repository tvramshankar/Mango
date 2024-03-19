using System;
using Mango.Services.OrderAPI.Models.DTO;
using Mango.Services.OrderAPI.Models.ResponseModel;
using Mango.Services.OrderAPI.Service.IService;
using Newtonsoft.Json;

namespace Mango.Services.OrderAPI.Service
{
    public class ProductService : IProductService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public ProductService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        public async Task<IEnumerable<ProductsDTO>> GetProducts()
        {
            var client = _httpClientFactory.CreateClient("Product");
            var responce = await client.GetAsync($"/api/ProductsAPI/GetAllProducts");
            var apiContent = await responce.Content.ReadAsStringAsync();
            var resp = JsonConvert.DeserializeObject<ServiceResponce<object>>(apiContent);
            if (resp!.IsSuccess)
                return JsonConvert.DeserializeObject<IEnumerable<ProductsDTO>>(resp.Data!.ToString()!)!;
            return new List<ProductsDTO>();
        }
    }
}