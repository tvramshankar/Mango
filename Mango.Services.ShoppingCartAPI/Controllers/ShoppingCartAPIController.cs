using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Mango.Services.ShoppingCartAPI.Data;
using Mango.Services.ShoppingCartAPI.Models;
using Mango.Services.ShoppingCartAPI.Models.DTO;
using Mango.Services.ShoppingCartAPI.Models.ResponseModel;
using Mango.Services.ShoppingCartAPI.Service.IService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Mango.Services.ShoppingCartAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShoppingCartAPIController : ControllerBase
    {
        private readonly ServiceResponce<object> _serviceResponce;
        private readonly IMapper _mapper;
        private readonly DataContext _dataContext;
        private readonly IProductService _productService;
        private readonly ICouponService _couponService;

        public ShoppingCartAPIController(IMapper mapper, DataContext dataContext,
            IProductService productService, ICouponService couponService)
        {
            _mapper = mapper;
            _dataContext = dataContext;
            _serviceResponce = new();
            _productService = productService;
            _couponService = couponService;
        }

        [HttpGet("GetCart/{userId}")]
        public async Task<ActionResult<ServiceResponce<object>>> GetCart(string userId)
        {
            try
            {
                CartDTO cart = new()
                {
                    CartHeader = _mapper.Map<CartHeaderDTO>(await _dataContext.CartHeaders.FirstOrDefaultAsync(e => e.UserId == userId))
                };
                cart.CartDetails = _mapper.Map<IEnumerable<CartDetailsDTO>>(_dataContext.CartDetails.Where(e=> e.CartHeaderId == cart.CartHeader.CartHeaderId));
                IEnumerable<ProductsDTO> products = await _productService.GetProducts();
                foreach(var item in cart.CartDetails)
                {
                    item.Product = products.FirstOrDefault(e=> e.ProductId == item.ProductId);
                    
                    cart.CartHeader.CartTotal += (item.Count * item.Product!.Price);
                }

                //apply coupon if any
                if(!string.IsNullOrEmpty(cart.CartHeader.CouponCode))
                {
                    CouponDTO coupon = await _couponService.GetCoupon(cart.CartHeader.CouponCode);
                    if(coupon is not null && cart.CartHeader.CartTotal > coupon.MinAmount)
                    {
                        cart.CartHeader.CartTotal -= coupon.DiscountAmount;
                        cart.CartHeader.Discount = coupon.DiscountAmount;
                    }
                }
                _serviceResponce.Data = cart;
            }
            catch(Exception ex)
            {
                _serviceResponce.Message = ex.Message;
                _serviceResponce.IsSuccess = false;
            }
            return Ok(_serviceResponce);
        }

        [HttpPost("CartUpsert")]
        public async Task<ActionResult<ServiceResponce<object>>> CartUpsert(CartDTO cartDTO)
        {
            try
            {
                var cartHeader = await _dataContext.CartHeaders
                    .AsNoTracking()
                    .FirstOrDefaultAsync(e=> e.UserId == cartDTO.CartHeader.UserId);
                if(cartHeader is null)
                {
                    //create header and details
                    CartHeader cartHeaderData = _mapper.Map<CartHeader>(cartDTO.CartHeader);
                    _dataContext.CartHeaders.Add(cartHeaderData);
                    await _dataContext.SaveChangesAsync();
                    cartDTO.CartDetails.First().CartHeaderId = cartHeaderData
                        .CartHeaderId;
                    CartDetails cartDetailsData = _mapper.Map<CartDetails>(cartDTO.CartDetails.First());
                    _dataContext.CartDetails.Add(cartDetailsData);
                    await _dataContext.SaveChangesAsync();
                }
                else
                {
                    //if header is not null
                    //check if details has same product
                    var cartDetails = await _dataContext
                        .CartDetails
                        .AsNoTracking()
                        .FirstOrDefaultAsync(u=> u.ProductId == cartDTO.CartDetails
                        .First()
                        .ProductId
                        && u.CartHeaderId == cartDTO.CartHeader.CartHeaderId);
                    if(cartDetails is null)
                    {
                        //create cartdetails
                        cartDTO.CartDetails.First().CartHeaderId = cartHeader.CartHeaderId;
                        CartDetails cartDetailsData = _mapper.Map<CartDetails>(cartDTO.CartDetails.First());
                        _dataContext.CartDetails.Add(cartDetailsData);
                        await _dataContext.SaveChangesAsync();
                    }
                    else
                    {
                        cartDTO.CartDetails.First().Count += cartDetails.Count;
                        cartDTO.CartDetails.First().CartHeaderId = cartDetails.CartHeaderId;
                        cartDTO.CartDetails.First().CartDetailsId += cartDetails.CartDetailsId;
                        _dataContext.CartDetails.Update(_mapper.Map<CartDetails>(cartDTO.CartDetails.First()));
                        await _dataContext.SaveChangesAsync();
                    }
                }
                _serviceResponce.Data = cartDTO;
            }
            catch(Exception ex)
            {
                _serviceResponce.Message = ex.Message;
                _serviceResponce.IsSuccess = false;
            }
            return Ok(_serviceResponce);
        }

        [HttpPost("RemoveCart")]
        public async Task<ActionResult<ServiceResponce<object>>> RemoveCart([FromBody] int cartDetailsId)
        {
            try
            {
                var cartDetails = await _dataContext.CartDetails
                    .AsNoTracking()
                    .FirstOrDefaultAsync(e => e.CartDetailsId == cartDetailsId);
                int totalCountOfCartItems = _dataContext.CartDetails.Where(e => e.CartHeaderId == cartDetails!.CartHeaderId).Count();
                _dataContext.CartDetails.Remove(cartDetails!);
                if (totalCountOfCartItems == 1)
                {
                    var cartHeaderToRemove = await _dataContext.CartHeaders
                        .FirstOrDefaultAsync(e => e.CartHeaderId == cartDetails!.CartHeaderId);
                    _dataContext.CartHeaders.Remove(cartHeaderToRemove!);
                }
                await _dataContext.SaveChangesAsync();
                _serviceResponce.Data = true;
            }
            catch (Exception ex)
            {
                _serviceResponce.Message = ex.Message;
                _serviceResponce.IsSuccess = false;
            }
            return Ok(_serviceResponce);
        }

        [HttpPost("ApplyCoupon")]
        public async Task<ActionResult<ServiceResponce<object>>> ApplyCoupon([FromBody] CartDTO cartDTO)
        {
            try
            {
                var cartFromDb = await _dataContext.CartHeaders.FirstOrDefaultAsync(u => u.UserId == cartDTO.CartHeader.UserId);
                cartFromDb!.CouponCode = cartDTO.CartHeader.CouponCode;
                _dataContext.CartHeaders.Update(cartFromDb);
                await _dataContext.SaveChangesAsync();
                _serviceResponce.Data = true;
            }
            catch (Exception ex)
            {
                _serviceResponce.Message = ex.Message;
                _serviceResponce.IsSuccess = false;
            }
            return Ok(_serviceResponce);
        }

        [HttpPost("RemoveCoupon")]
        public async Task<ActionResult<ServiceResponce<object>>> RemoveCoupon([FromBody] CartDTO cartDTO)
        {
            try
            {
                var cartFromDb = await _dataContext.CartHeaders.FirstOrDefaultAsync(u => u.UserId == cartDTO.CartHeader.UserId);
                cartFromDb!.CouponCode = string.Empty;
                _dataContext.CartHeaders.Update(cartFromDb);
                await _dataContext.SaveChangesAsync();
                _serviceResponce.Data = true;
            }
            catch (Exception ex)
            {
                _serviceResponce.Message = ex.Message;
                _serviceResponce.IsSuccess = false;
            }
            return Ok(_serviceResponce);
        }
    }
}
