using System;
using Mango.Web.Models;
using Mango.Web.Models.DTO;
namespace Mango.Web.Service.IService;

public interface ICouponService
{
    Task<ServiceResponce<object>> GetAllCouponAsync();
    Task<ServiceResponce<object>> GetCouponByCodeAsync(string couponId);
    Task<ServiceResponce<object>> GetCouponByIdAsync(int Id);
    Task<ServiceResponce<object>> CreateCouponAsync(CouponPostDTO coupon);
    Task<ServiceResponce<object>> UpdateCouponAsync(CouponPostDTO coupon);
    Task<ServiceResponce<object>> DeleteCouponAsync(int Id);
}

