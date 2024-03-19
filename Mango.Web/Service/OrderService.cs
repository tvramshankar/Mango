using System;
using Mango.Web.Models;
using Mango.Web.Models.DTO;
using Mango.Web.Service.IService;
namespace Mango.Web.Service
{
	public class OrderService : IOrderService
	{
		private readonly IBaseService _baseService;
		public OrderService(IBaseService baseService)
		{
			_baseService = baseService;
		}

        public async Task<ServiceResponce<object>> CreateOrder(CartDTO cartDTO)
        {
            return await _baseService.SendAsync(new ServiceRequest
            {
                ApiType = Utility.StaticDetails.ApiType.POST,
                Url = Utility.StaticDetails.OrderAPIBase + "/api/Order/CreateOrder",
                Data = cartDTO,
            });
        }

        public async Task<ServiceResponce<object>> CreateStripeSession(StripeRequestDTO stripeRequestDTO)
        {
            return await _baseService.SendAsync(new ServiceRequest
            {
                ApiType = Utility.StaticDetails.ApiType.POST,
                Url = Utility.StaticDetails.OrderAPIBase + "/api/Order/CreateStripeSession",
                Data = stripeRequestDTO,
            });
        }

        public async Task<ServiceResponce<object>> ValidateStripeSession(int orderHeaderId)
        {
            return await _baseService.SendAsync(new ServiceRequest
            {
                ApiType = Utility.StaticDetails.ApiType.POST,
                Url = Utility.StaticDetails.OrderAPIBase + "/api/Order/ValidateStripeSession",
                Data = orderHeaderId,
            });
        }
    }
}

