using System;
using Mango.Web.Models;
using Mango.Web.Models.DTO;
using Mango.Web.Service.IService;
namespace Mango.Web.Service
{
	public class CouponService : ICouponService
	{
		private readonly IBaseService _baseService;
		public CouponService(IBaseService baseService)
		{
			_baseService = baseService;
		}

        public async Task<ServiceResponce<object>> GetAllCouponAsync()
        {
            return await _baseService.SendAsync(new ServiceRequest
            {
                ApiType = Utility.StaticDetails.ApiType.GET,
                Url = Utility.StaticDetails.CouponAPIBase+ "/api/CouponAPI/GetAllCoupon",
            });
        }

        public async Task<ServiceResponce<object>> GetCouponByIdAsync(int Id)
        {
            return await _baseService.SendAsync(new ServiceRequest
            {
                ApiType = Utility.StaticDetails.ApiType.GET,
                Url = Utility.StaticDetails.CouponAPIBase + "/api/CouponAPI/GetById",
            });
        }

        public async Task<ServiceResponce<object>> GetCouponByCodeAsync(string couponId)
        {
            return await _baseService.SendAsync(new ServiceRequest
            {
                ApiType = Utility.StaticDetails.ApiType.GET,
                Url = Utility.StaticDetails.CouponAPIBase + "/api/CouponAPI/GetByCode",
            });
        }

        public async Task<ServiceResponce<object>> CreateCouponAsync(CouponPostDTO coupon)
        {
            return await _baseService.SendAsync(new ServiceRequest
            {
                ApiType = Utility.StaticDetails.ApiType.GET,
                Url = Utility.StaticDetails.CouponAPIBase + "/api/CouponAPI/AddCoupon",
            });
        }

        public async Task<ServiceResponce<object>> UpdateCouponAsync(CouponPostDTO coupon)
        {
            return await _baseService.SendAsync(new ServiceRequest
            {
                ApiType = Utility.StaticDetails.ApiType.GET,
                Url = Utility.StaticDetails.CouponAPIBase + "/api/CouponAPI/UpdateCoupon",
            });
        }

        public async Task<ServiceResponce<object>> DeleteCouponAsync(int Id)
        {
            return await _baseService.SendAsync(new ServiceRequest
            {
                ApiType = Utility.StaticDetails.ApiType.GET,
                Url = Utility.StaticDetails.CouponAPIBase + "/api/CouponAPI/DeleteCoupon",
            });
        }
    }
}

