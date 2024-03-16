using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using Mango.Web.Models;
using Mango.Web.Models.DTO;
using Mango.Web.Service.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Mango.Web.Controllers
{
    public class CartController : Controller
    {
        private readonly ICartService _cartService;
        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        [Authorize]
        public async Task<IActionResult> CartIndex()
        {
            return View(await LoadCartDtoBasedOnLoggedInUser());
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