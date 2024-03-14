using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Mango.Web.Models;
using Newtonsoft.Json;
using Mango.Web.Models.DTO;
using Mango.Web.Service.IService;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;

namespace Mango.Web.Controllers;

public class HomeController : Controller
{
    private readonly IProductService _productsService;
    private readonly ICartService _cartService;

    public HomeController(ILogger<HomeController> logger, IProductService productService,
        ICartService cartService)
    {
        _productsService = productService;
        _cartService = cartService;
    }

    public async Task<IActionResult> Index()
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
        catch (Exception ex)
        {
            responceData.IsSuccess = false;
            responceData.Message = ex.Message;
            TempData["error"] = responceData.Message;
        }
        return View(responceData);
    }

    [Authorize]
    public async Task<IActionResult> Details(Guid productId)
    {
        ProductsDTO data = new();
        try
        {
            ServiceResponce<object> responce = await _productsService.GetProductByIdAsync(productId);
            if (responce is not null && responce.IsSuccess)
            {
                data = JsonConvert.DeserializeObject<ProductsDTO>(responce.Data!.ToString()!)!;
            }
            else
            {
                TempData["error"] = responce!.Message;
            }
        }
        catch
        {
            TempData["error"] = "Something went wrong";
        }
        return View(data);
    }

    [Authorize]
    [HttpPost]
    [ActionName("Details")]
    public async Task<IActionResult> Details(ProductsDTO productsDTO)
    {
        CartDTO cartDTO = new CartDTO()
        {
            CartHeader = new CartHeaderDTO
            {
                UserId = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Sub)?.FirstOrDefault()?.Value,
            }
        };

        CartDetailsDTO cartDetails = new CartDetailsDTO()
        {
            Count = productsDTO.ProductCount,
            ProductId = productsDTO.ProductId,
        };

        List<CartDetailsDTO> cartDetailsDTOs = new() { cartDetails };
        cartDTO.CartDetails = cartDetailsDTOs;
        
        try
        {
            ServiceResponce<object> responce = await _cartService.CartUpsert(cartDTO);
            if (responce is not null && responce.IsSuccess)
            {
                TempData["success"] = "Item has been added to shopping cart";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                TempData["error"] = responce!.Message;
            }
        }
        catch
        {
            TempData["error"] = "Something went wrong";
        }
        return View(productsDTO);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

