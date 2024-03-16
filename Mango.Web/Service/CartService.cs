using System;
using Mango.Web.Models;
using Mango.Web.Models.DTO;
using Mango.Web.Service.IService;
namespace Mango.Web.Service
{
	public class CartService : ICartService
	{
		private readonly IBaseService _baseService;
		public CartService(IBaseService baseService)
		{
			_baseService = baseService;
		}

        public async Task<ServiceResponce<object>> ApplyCoupon(CartDTO cartDTO)
        {
            return await _baseService.SendAsync(new ServiceRequest
            {
                ApiType = Utility.StaticDetails.ApiType.POST,
                Url = Utility.StaticDetails.ShoppingCartAPIBase + "/api/ShoppingCartAPI/ApplyCoupon",
                Data = cartDTO
            });
        }

        public async Task<ServiceResponce<object>> CartUpsert(CartDTO cartDTO)
        {
            return await _baseService.SendAsync(new ServiceRequest
            {
                ApiType = Utility.StaticDetails.ApiType.POST,
                Url = Utility.StaticDetails.ShoppingCartAPIBase + "/api/ShoppingCartAPI/CartUpsert",
                Data = cartDTO
            });
        }

        public async Task<ServiceResponce<object>> EmailCart(CartDTO cartDTO)
        {
            return await _baseService.SendAsync(new ServiceRequest
            {
                ApiType = Utility.StaticDetails.ApiType.POST,
                Url = Utility.StaticDetails.ShoppingCartAPIBase + "/api/ShoppingCartAPI/EmailCartRequest",
                Data = cartDTO
            });
        }

        public async Task<ServiceResponce<object>> GetCart(string userId)
        {
            return await _baseService.SendAsync(new ServiceRequest
            {
                ApiType = Utility.StaticDetails.ApiType.GET,
                Url = Utility.StaticDetails.ShoppingCartAPIBase + "/api/ShoppingCartAPI/GetCart" + $"/{userId}",
            });
        }

        public async Task<ServiceResponce<object>> RemoveCart(int cartDetailsId)
        {
            return await _baseService.SendAsync(new ServiceRequest
            {
                ApiType = Utility.StaticDetails.ApiType.POST,
                Url = Utility.StaticDetails.ShoppingCartAPIBase + "/api/ShoppingCartAPI/RemoveCart",
                Data = cartDetailsId
            });
        }

        public async Task<ServiceResponce<object>> RemoveCoupon(CartDTO cartDTO)
        {
            return await _baseService.SendAsync(new ServiceRequest
            {
                ApiType = Utility.StaticDetails.ApiType.POST,
                Url = Utility.StaticDetails.ShoppingCartAPIBase + "/api/ShoppingCartAPI/RemoveCoupon",
                Data = cartDTO
            });
        }
    }
}

