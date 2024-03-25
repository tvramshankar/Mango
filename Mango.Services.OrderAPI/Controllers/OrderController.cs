using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Mango.MessageBus;
using Mango.Services.OrderAPI.Data;
using Mango.Services.OrderAPI.Model;
using Mango.Services.OrderAPI.Model.DTO;
using Mango.Services.OrderAPI.Models.DTO;
using Mango.Services.OrderAPI.Models.ResponseModel;
using Mango.Services.OrderAPI.Service.IService;
using Mango.Services.OrderAPI.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stripe;
using Stripe.Checkout;

namespace Mango.Services.OrderAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly DataContext _dataContext;
        private readonly IProductService _productService;
        private readonly IMapper _mapper;
        private readonly ServiceResponce<object> _serviceResponce;
        private readonly IMessageBus _messageBus;
        private readonly IConfiguration _configuration;
        public OrderController(DataContext dataContext, IProductService productService,
            IMapper mapper, IMessageBus messageBus, IConfiguration configuration)
        {
            _dataContext = dataContext;
            _productService = productService;
            _mapper = mapper;
            _serviceResponce = new();
            _messageBus = messageBus;
            _configuration = configuration;
        }

        [Authorize]
        [HttpGet("GetOrdersByUserId/{userId}")]
        public async Task<ActionResult<ServiceResponce<object>>> GetOrdersByUserId(string userId="")
        {
            try
            {
                IEnumerable<OrderHeader> orderHeaders;
                if (User.IsInRole(StaticDetails.RoleAdmin))
                {
                    orderHeaders = await _dataContext.OrderHeaders
                        .Include(u => u.orderDetails)
                        .OrderByDescending(u => u.OrderHeaderId).ToListAsync();
                    _serviceResponce.Data = orderHeaders;
                }
                else
                {
                    orderHeaders = await _dataContext.OrderHeaders
                        .Include(u => u.orderDetails)
                        .Where(u => u.UserId == userId)
                        .OrderByDescending(u => u.OrderHeaderId).ToListAsync();
                    _serviceResponce.Data = orderHeaders;
                }
            }
            catch(Exception ex)
            {
                _serviceResponce.IsSuccess = false;
                _serviceResponce.Message = ex.Message;
            }
            return _serviceResponce;
        }


        [Authorize]
        [HttpGet("GetOrdersByOrderId/{orderId}")]
        public async Task<ActionResult<ServiceResponce<object>>> GetOrdersByOrderId(int orderId)
        {
            try
            {
                OrderHeader orderHeader = _dataContext.OrderHeaders
                    .Include(u => u.orderDetails)
                    .First(u => u.OrderHeaderId == orderId);
                _serviceResponce.Data = orderHeader;
            }
            catch (Exception ex)
            {
                _serviceResponce.IsSuccess = false;
                _serviceResponce.Message = ex.Message;
            }
            return _serviceResponce;
        }

        [Authorize]
        [HttpPost("CreateOrder")]
        public async Task<ActionResult<ServiceResponce<object>>> CreateOrder([FromBody] CartDTO cartDTO)
        {
            try
            {
                OrderHeaderDTO orderHeaderDTO = _mapper.Map<OrderHeaderDTO>(cartDTO.CartHeader);
                orderHeaderDTO.OrderTime = DateTime.Now;
                orderHeaderDTO.Status = StaticDetails.Status_Pending;
                orderHeaderDTO.orderDetails = _mapper.Map<IEnumerable<OrderDetailsDTO>>(cartDTO.CartDetails);

                OrderHeader orderCreated = _dataContext.OrderHeaders.Add(_mapper.Map<OrderHeader>(orderHeaderDTO)).Entity;
                await _dataContext.SaveChangesAsync();
                orderHeaderDTO.OrderHeaderId = orderCreated.OrderHeaderId;
                _serviceResponce.Data = orderHeaderDTO;
            }
            catch (Exception ex)
            {
                _serviceResponce.IsSuccess = false;
                _serviceResponce.Message = ex.ToString();
            }
            return _serviceResponce;
        }

        [Authorize]
        [HttpPost("CreateStripeSession")]
        public async Task<ActionResult<ServiceResponce<object>>> CreateStripeSession([FromBody] StripeRequestDTO stripeRequestDTO)
        {
            try
            {
                var options = new Stripe.Checkout.SessionCreateOptions
                {
                    SuccessUrl = stripeRequestDTO.ApprovedUrl,
                    CancelUrl = stripeRequestDTO.CancelUrl,
                    LineItems = new List<Stripe.Checkout.SessionLineItemOptions>(),
                    Mode = "payment",
                    //Customer = stripeRequestDTO.OrderHeader.Name,
                   // CustomerEmail = stripeRequestDTO.OrderHeader.Email,
                   // BillingAddressCollection = stripeRequestDTO.OrderHeader.Name,
                };

                var DiscountDetails = new List<SessionDiscountOptions>()
                {
                    new SessionDiscountOptions
                    {
                        Coupon = stripeRequestDTO.OrderHeader.CouponCode
                    }
                };

                foreach (var item in stripeRequestDTO.OrderHeader.orderDetails)
                {
                    var sessionLineItem = new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = (long)(item.Price * 100),
                            Currency = "usd",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = item.ProductName
                            },

                        },
                        Quantity = item.Count,
                    };
                    options.LineItems.Add(sessionLineItem);
                  //  options.ShippingAddressCollection = billingAddressDetails;
                }

                if(stripeRequestDTO.OrderHeader.Discount > 0)
                {
                    options.Discounts = DiscountDetails;
                }
                var service = new Stripe.Checkout.SessionService();
                Session session = service.Create(options);
                stripeRequestDTO.StripeSessionUrl = session.Url;
                OrderHeader orderHeader = _dataContext.OrderHeaders.First(u => u.OrderHeaderId == stripeRequestDTO.OrderHeader.OrderHeaderId);
                orderHeader.StripeSessionId = session.Id;
                await _dataContext.SaveChangesAsync();
                _serviceResponce.Data = stripeRequestDTO;
            }
            catch(Exception ex)
            {
                _serviceResponce.IsSuccess = false;
                _serviceResponce.Message = ex.Message;
            }
            return _serviceResponce;
        }

        [Authorize]
        [HttpPost("ValidateStripeSession")]
        public async Task<ActionResult<ServiceResponce<object>>> ValidateStripeSession([FromBody] int orderHeaderId)
        {
            try
            {
                OrderHeader orderHeader = _dataContext.OrderHeaders.First(u => u.OrderHeaderId == orderHeaderId);

                var service = new Stripe.Checkout.SessionService();
                Session session = await service.GetAsync(orderHeader.StripeSessionId);

                var paymentIntentService = new PaymentIntentService();
                PaymentIntent paymentIntent = await paymentIntentService.GetAsync(session.PaymentIntentId);

                if(paymentIntent.Status == "succeeded")
                {
                    //then payment was successfull
                    orderHeader.PaymentIntentId = paymentIntent.Id;
                    orderHeader.Status = StaticDetails.Status_Approved;
                    await _dataContext.SaveChangesAsync();
                    RewardDTO rewardDTO = new()
                    {
                        OrderId = orderHeader.OrderHeaderId,
                        UserId = orderHeader.UserId!,
                        RewardActivity = Convert.ToInt32(orderHeader.OrderTotal)
                    };
                    string topicName = _configuration.GetSection("TopicAndQueueNames:OrderCreatedTopic").Get<string>()!;
                    await _messageBus.PublishMessage(rewardDTO, topicName);
                    _serviceResponce.Data = _mapper.Map<OrderHeaderDTO>(orderHeader);
                }
            }
            catch (Exception ex)
            {
                _serviceResponce.IsSuccess = false;
                _serviceResponce.Message = ex.Message;
            }
            return _serviceResponce;
        }

        [Authorize]
        [HttpPost("UpdateOrderStatus/{orderId}")]
        public async Task<ActionResult<ServiceResponce<object>>> UpdateOrderStatus(int orderId, [FromBody] string newStatus)
        {
            try
            {
                OrderHeader orderHeader = _dataContext.OrderHeaders.First(u => u.OrderHeaderId == orderId);
                if(orderHeader is not null)
                {
                    if(newStatus == StaticDetails.Status_Cancelled)
                    {
                        //we will gice refund
                        var options = new RefundCreateOptions
                        {
                            Reason = RefundReasons.RequestedByCustomer,
                            PaymentIntent = orderHeader.PaymentIntentId,
                        };

                        var service = new RefundService();
                        Refund refund = await service.CreateAsync(options);
                    }
                    orderHeader.Status = newStatus;
                    await _dataContext.SaveChangesAsync();
                }
            }
            catch(Exception ex)
            {
                _serviceResponce.Message = ex.Message;
                _serviceResponce.IsSuccess = false;
            }
            return _serviceResponce;
        }
    }
}
