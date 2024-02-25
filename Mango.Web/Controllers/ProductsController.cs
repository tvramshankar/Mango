using System;
using Mango.Web.Models;
using Mango.Web.Models.DTO;
using Mango.Web.Service.IService;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Mango.Web.Controllers
{
	public class ProductsController : Controller
	{

        private readonly ILogger<HomeController> _logger;
        private readonly IProductService _productsService;

        public ProductsController(ILogger<HomeController> logger, IProductService productService)
        {
            _productsService = productService;
            _logger = logger;
        }

        public async Task<IActionResult> ProductIndex()
        {
            ServiceResponce<List<ProductsDTO>> responceData = new();
            try
            {
                ServiceResponce<object> responce = await _productsService.GetAllProductAsync();
                if (responce is not null && responce.IsSuccess)
                {
                    responceData.Data = JsonConvert.DeserializeObject<List<ProductsDTO>>(responce.Data!.ToString()!);
                }
                else
                {
                    TempData["error"] = responce!.Message;
                }
            }
            catch(Exception ex)
            {
                responceData.IsSuccess = false;
                responceData.Message = ex.Message;
                TempData["error"] = responceData.Message;
            }
            return View(responceData);
        }

        public async Task<IActionResult> ProductCreate()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ProductCreate(ProductPostDTO productPostDTO)
        {
            ServiceResponce<ProductPostDTO> responceData = new();
            try
            {
                if(ModelState.IsValid)
                {
                    ServiceResponce<object> responce = await _productsService.CreateProductAsync(productPostDTO);
                    if (responce is not null && responce.IsSuccess)
                    {
                        TempData["success"] = "Success";
                        return RedirectToAction(nameof(ProductIndex));
                    }
                }
                throw new Exception($"something went wrong at {nameof(ProductsController)} at method {nameof(ProductCreate)} Post");
            }
            catch (Exception ex)
            {
                responceData.IsSuccess = false;
                responceData.Message = ex.Message;
                TempData["error"] = responceData.Message;
            }
            return View();
        }

        public async Task<IActionResult> ProductDelete(Guid Id)
        {
            ServiceResponce<ProductsDTO> responceData = new();
            try
            {
                ServiceResponce<object> responce = await _productsService.DeleteProductAsync(Id);
                if (responce is not null && responce.IsSuccess)
                {
                    TempData["success"] = "Success";
                    return RedirectToAction(nameof(ProductIndex));   
                }
                throw new Exception(responce!.Message);
            }
            catch(Exception ex)
            {
                responceData.IsSuccess = false;
                responceData.Message = ex.Message;
                TempData["error"] = responceData.Message;
            }
            return RedirectToAction(nameof(ProductIndex));
        }
    }
}

