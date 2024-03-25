using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using Mango.Web.Models;
using Mango.Web.Models.DTO;
using Mango.Web.Service.IService;
using Mango.Web.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Mango.Web.Controllers
{
    public class CartController : Controller
    {
        private readonly ICartService _cartService;
        private readonly IOrderService _orderService;
        public CartController(ICartService cartService, IOrderService orderService)
        {
            _cartService = cartService;
            _orderService = orderService;
        }

        [Authorize]
        public async Task<IActionResult> CartIndex()
        {
            return View(await LoadCartDtoBasedOnLoggedInUser());
        }

        [Authorize]
        public async Task<IActionResult> CheckOut()
        {
            return View(await LoadCartDtoBasedOnLoggedInUser());
        }

        [Authorize]
        public async Task<IActionResult> Confirmation(int orderId)
        {
            ServiceResponce<object> response = await _orderService.ValidateStripeSession(orderId);
            if (response is not null && response.IsSuccess)
            {
                OrderHeaderDTO orderHeader = JsonConvert.DeserializeObject<OrderHeaderDTO>(response.Data!.ToString()!)!;
                if(orderHeader.Status == StaticDetails.Status_Approved)
                    return View(orderId);
            }

            //here we can redirect to page based on status
            return View(orderId);
        }

        [Authorize]
        [HttpPost]
        [ActionName("CheckOut")]
        public async Task<IActionResult> CheckOut(CartDTO cartDTO)
        {
            CartDTO cart = await LoadCartDtoBasedOnLoggedInUser();
            cart.CartHeader.Name = cartDTO.CartHeader.Name;
            cart.CartHeader.Phone = cartDTO.CartHeader.Phone;
            cart.CartHeader.Email = cartDTO.CartHeader.Email;

            var responce = await _orderService.CreateOrder(cart);
            
            if(responce is not null && responce.IsSuccess)
            {
                OrderHeaderDTO orderHeaderDTO = JsonConvert.DeserializeObject<OrderHeaderDTO>(Convert.ToString(responce.Data)!)!;

                //get stripe session and redirect to stripe to place order

                var domain = $"{Request.Scheme}://{Request.Host.Value}/";

                StripeRequestDTO stripeRequestDTO = new()
                {
                    OrderHeader = orderHeaderDTO,
                    ApprovedUrl = $"{domain}Cart/Confirmation?orderId={orderHeaderDTO.OrderHeaderId}",
                    CancelUrl = $"{domain}cart/checkout",
                };

                var stripeResponce = await _orderService.CreateStripeSession(stripeRequestDTO);
                StripeRequestDTO stripeData = JsonConvert.DeserializeObject<StripeRequestDTO>(Convert.ToString(stripeResponce.Data)!)!;
                Response.Headers.Add("Location", stripeData.StripeSessionUrl);
                return new StatusCodeResult(303);
            }
            return View();
        }

        public async Task<IActionResult> Remove(int cartDetailsId)
        {
            ServiceResponce<object> response = await _cartService.RemoveCart(cartDetailsId);
            if (response is not null && response.IsSuccess)
            {
                TempData["success"] = "Cart updated successfully";
                return RedirectToAction(nameof(CartIndex));
            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ApplyCoupon(CartDTO cart)
        {
            ServiceResponce<object> response = await _cartService.ApplyCoupon(cart);
            if (response is not null && response.IsSuccess)
            {
                TempData["success"] = "Cart updated successfully";
                return RedirectToAction(nameof(CartIndex));
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> EmailCart(CartDTO cart)
        {
            CartDTO cartDto = await LoadCartDtoBasedOnLoggedInUser();
            cartDto.CartHeader.Email = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Email)?.FirstOrDefault()?.Value;
            ServiceResponce<object> response = await _cartService.EmailCart(cartDto);
            if (response is not null && response.IsSuccess)
            {
                TempData["success"] = "Email will be processed and will be send shortly";
                return RedirectToAction(nameof(CartIndex));
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RemoveCoupon(CartDTO cart)
        {
            ServiceResponce<object> response = await _cartService.RemoveCoupon(cart);
            if (response is not null && response.IsSuccess)
            {
                TempData["success"] = "Cart updated successfully";
                return RedirectToAction(nameof(CartIndex));
            }
            return View();
        }

        private async Task<CartDTO> LoadCartDtoBasedOnLoggedInUser()
        {
            var userId = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Sub)?.FirstOrDefault()?.Value;
            ServiceResponce<object> response = await _cartService.GetCart(userId!);
            if(response is not null && response.IsSuccess)
            {
                CartDTO cartDTO = JsonConvert.DeserializeObject<CartDTO>(Convert.ToString(response.Data)!)!;
                return cartDTO;
            }
            return new();
        }
    }
}