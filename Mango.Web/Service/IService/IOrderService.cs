using System;
using Mango.Web.Models;
using Mango.Web.Models.DTO;
namespace Mango.Web.Service.IService;

public interface IOrderService
{
    Task<ServiceResponce<object>> CreateOrder(CartDTO cartDTO);
    Task<ServiceResponce<object>> CreateStripeSession(StripeRequestDTO stripeRequestDTO);
    Task<ServiceResponce<object>> ValidateStripeSession(int orderHeaderId);
}

