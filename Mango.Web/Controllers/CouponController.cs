using System;
using Mango.Web.Models;
using Mango.Web.Models.DTO;
using Mango.Web.Service.IService;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Mango.Web.Controllers
{
	public class CouponController : Controller
	{

        private readonly ILogger<HomeController> _logger;
        private readonly ICouponService _couponService;

        public CouponController(ILogger<HomeController> logger, ICouponService couponService)
        {
            _couponService = couponService;
            _logger = logger;
        }

        public async Task<IActionResult> CouponIndex()
        {
            ServiceResponce<List<CouponDTO>> responceData = new();
            try
            {
                ServiceResponce<object> responce = await _couponService.GetAllCouponAsync();
                if (responce is not null && responce.IsSuccess)
                {
                    responceData.Data = JsonConvert.DeserializeObject<List<CouponDTO>>(responce.Data!.ToString()!);
                }
            }
            catch(Exception ex)
            {
                responceData.IsSuccess = false;
                responceData.Message = ex.Message;
            }
            return View(responceData);
        }
    }
}

