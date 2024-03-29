using System;
using Mango.Web.Models;
using Mango.Web.Models.DTO;
using Mango.Web.Service.IService;
namespace Mango.Web.Service
{
	public class ProductService : IProductService
	{
		private readonly IBaseService _baseService;
		public ProductService(IBaseService baseService)
		{
			_baseService = baseService;
		}

        public async Task<ServiceResponce<object>> GetAllProductAsync()
        {
            return await _baseService.SendAsync(new ServiceRequest
            {
                ApiType = Utility.StaticDetails.ApiType.GET,
                Url = Utility.StaticDetails.ProductAPIBase+ "/api/ProductsAPI/GetAllProducts",
            });
        }

        public async Task<ServiceResponce<object>> GetProductByIdAsync(Guid Id)
        {
            return await _baseService.SendAsync(new ServiceRequest
            {
                ApiType = Utility.StaticDetails.ApiType.GET,
                Url = Utility.StaticDetails.ProductAPIBase + "/api/ProductsAPI/GetById/" + Id,
            });
        }

        public async Task<ServiceResponce<object>> CreateProductAsync(ProductPostDTO product)
        {
            return await _baseService.SendAsync(new ServiceRequest
            {
                ApiType = Utility.StaticDetails.ApiType.POST,
                Url = Utility.StaticDetails.ProductAPIBase + "/api/ProductsAPI/AddProduct",
                Data = product,
                ContentType = Utility.StaticDetails.ContentType.MultipartFormData
            });
        }

        public async Task<ServiceResponce<object>> UpdateProductAsync(ProductsDTO product)
        {
            return await _baseService.SendAsync(new ServiceRequest
            {
                ApiType = Utility.StaticDetails.ApiType.PUT,
                Url = Utility.StaticDetails.ProductAPIBase + "/api/ProductsAPI/UpdateProduct",
                Data = product,
                ContentType = Utility.StaticDetails.ContentType.MultipartFormData
            });
        }

        public async Task<ServiceResponce<object>> DeleteProductAsync(Guid Id)
        {
            return await _baseService.SendAsync(new ServiceRequest
            {
                ApiType = Utility.StaticDetails.ApiType.DELETE,
                Url = Utility.StaticDetails.ProductAPIBase + "/api/ProductsAPI/DeleteProduct/" + Id,
            });
        }
    }
}

