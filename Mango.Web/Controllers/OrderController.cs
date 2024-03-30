using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using Mango.Web.Models.DTO;
using Mango.Web.Models;
using Mango.Web.Service.IService;
using Mango.Web.Utility;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Mango.Web.Controllers
{
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;
        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }
        public IActionResult OrderIndex()
        {
            return View();
        }

        public async Task<IActionResult> OrderDetail(int orderId)
        {
            OrderHeaderDTO orderHeaderDTO = new OrderHeaderDTO();
            string userId = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Sub)?.FirstOrDefault()?.Value!;
            var serviceResponce = await _orderService.GetOrdersByOrderId(orderId);
            if (serviceResponce is not null && serviceResponce.IsSuccess)
            {
                orderHeaderDTO = JsonConvert.DeserializeObject<OrderHeaderDTO>(Convert.ToString(serviceResponce.Data)!)!;
            }
            if(!User.IsInRole(StaticDetails.RoleAdmin) && userId != orderHeaderDTO.UserId)
            {
                return NotFound();
            }
            return View(orderHeaderDTO);
        }

        [HttpPost("OrderReadyForPickup")]
        public async Task<IActionResult> OrderReadyForPickup(int orderId)
        {
            var serviceResponce = await _orderService.UpdateOrderStatus(orderId, StaticDetails.Status_ReadyForPickup);
            if (serviceResponce is not null && serviceResponce.IsSuccess)
            {
                TempData["success"] = "status updated successfully";
                return RedirectToAction(nameof(OrderDetail), new { orderId = orderId });
            }
            return View();
        }

        [HttpPost("CompleteOrder")]
        public async Task<IActionResult> CompleteOrder(int orderId)
        {
            var serviceResponce = await _orderService.UpdateOrderStatus(orderId, StaticDetails.Status_Completed);
            if (serviceResponce is not null && serviceResponce.IsSuccess)
            {
                TempData["success"] = "status updated successfully";
                return RedirectToAction(nameof(OrderDetail), new { orderId = orderId });
            }
            return View();
        }

        [HttpPost("CancelOrder")]
        public async Task<IActionResult> CancelOrder(int orderId)
        {
            var serviceResponce = await _orderService.UpdateOrderStatus(orderId, StaticDetails.Status_Cancelled);
            if (serviceResponce is not null && serviceResponce.IsSuccess)
            {
                TempData["success"] = "status updated successfully";
                return RedirectToAction(nameof(OrderDetail), new { orderId = orderId });
            }
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(string status)
        {
            IEnumerable<OrderHeaderDTO> orderHeaderDTOs;
            string userId = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Sub)?.FirstOrDefault()?.Value!;
            ServiceResponce<object> serviceResponce = await _orderService.GetOrdersByUserId(userId);
            if(serviceResponce is not null && serviceResponce.IsSuccess)
            {
                orderHeaderDTOs = JsonConvert.DeserializeObject<List<OrderHeaderDTO>>(Convert.ToString(serviceResponce.Data)!)!;
                switch(status)
                {
                    case "approved":
                        orderHeaderDTOs = orderHeaderDTOs.Where(u => u.Status == StaticDetails.Status_Approved);
                        break;
                    case "readyforpickup":
                        orderHeaderDTOs = orderHeaderDTOs.Where(u => u.Status == StaticDetails.Status_ReadyForPickup);
                        break;
                    case "Cancelled":
                        orderHeaderDTOs = orderHeaderDTOs.Where(u => u.Status == StaticDetails.Status_Cancelled);
                        break;
                    default:
                        break;
                }
            }
            else
            {
                orderHeaderDTOs = new List<OrderHeaderDTO>();
            }
            return Json(new { data = orderHeaderDTOs });
        }
    }
}