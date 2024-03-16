using System;
using Mango.Web.Models;
using Mango.Web.Models.DTO;
namespace Mango.Web.Service.IService;

public interface ICartService
{
    Task<ServiceResponce<object>> GetCart(string userId);
    Task<ServiceResponce<object>> CartUpsert(CartDTO cartDTO);
    Task<ServiceResponce<object>> RemoveCart(int cartDetailsId);
    Task<ServiceResponce<object>> ApplyCoupon(CartDTO cartDTO);
    Task<ServiceResponce<object>> RemoveCoupon(CartDTO cartDTO);
    Task<ServiceResponce<object>> EmailCart(CartDTO cartDTO);
}