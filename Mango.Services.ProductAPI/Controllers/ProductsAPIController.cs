using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Mango.Services.ProductAPI.Data;
using Mango.Services.ProductAPI.Models;
using Mango.Services.ProductAPI.Models.DTO;
using Mango.Services.ProductAPI.Models.ResponseModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Mango.Services.ProductAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProductsAPIController : ControllerBase
    {
        private readonly DataContext _dataContext;
        private readonly IMapper _autoMapper;
        public ProductsAPIController(DataContext dataContext, IMapper autoMapper)
        {
            _dataContext = dataContext;
            _autoMapper = autoMapper;
        }

        [HttpGet("GetAllProducts")]
        public async Task<ActionResult<ServiceResponce<List<ProductsDTO>>>> Get()
        {
            var responce = new ServiceResponce<List<ProductsDTO>>();
            try
            {
                var data = await _dataContext.products.ToListAsync();
                responce.Data = _autoMapper.Map<List<ProductsDTO>>(data);
            }
            catch (Exception Ex)
            {
                responce.Message = Ex.Message;
                responce.IsSuccess = false;
            }
            return Ok(responce);
        }

        [HttpGet("GetById/{Id}")]
        public async Task<ActionResult<ServiceResponce<ProductsDTO>>> GetProductById(Guid Id)
        {
            var responce = new ServiceResponce<ProductsDTO>();
            try
            {
                var data = await _dataContext
                .products
                .FirstOrDefaultAsync(e => e.ProductId == Id);
                responce.Data = _autoMapper.Map<ProductsDTO>(data);
            }
            catch (Exception Ex)
            {
                responce.Message = Ex.Message;
                responce.IsSuccess = false;
            }
            return Ok(responce);
        }

        [HttpPost("AddProduct")]
        [Authorize(Roles = "ADMIN")]
        public async Task<ActionResult<ServiceResponce<ProductsDTO>>> AddProduct([FromForm] ProductPostDTO products)
        {
            var responce = new ServiceResponce<ProductsDTO>();
            try
            {
                var product = _autoMapper.Map<Product>(products);
                _dataContext.products.Add(product);
                await _dataContext.SaveChangesAsync();
                if(products.Image is not null)
                {
                    string fileName = product.ProductId + Path.GetExtension(products.Image.FileName);
                    string filePath = @"wwwroot/ProductImages/" + fileName;
                    var filePathDirectory = Path.Combine(Directory.GetCurrentDirectory(), filePath);
                    using (var fileStream = new FileStream(filePathDirectory,FileMode.Create))
                    {
                        await products.Image.CopyToAsync(fileStream);
                    }
                    var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.Value}{HttpContext.Request.PathBase.Value}";
                    product.ImageLocalPath = filePath;
                    product.ImageUrl = baseUrl + "/ProductImages/" + fileName;
                }
                else
                {
                    product.ImageUrl = "https://placehold.co/600x400";
                }

                _dataContext.products.Update(product);
                await _dataContext.SaveChangesAsync();
                responce.Data = _autoMapper.Map<ProductsDTO>(product);
            }
            catch (Exception Ex)
            {
                responce.Message = Ex.Message;
                responce.IsSuccess = false;
            }
            return Ok(responce);
        }

        [HttpPut("UpdateProduct")]
        [Authorize(Roles = "ADMIN")]
        public async Task<ActionResult<ServiceResponce<ProductsDTO>>> UpdateProduct(ProductsDTO product)
        {
            var responce = new ServiceResponce<ProductsDTO>();
            try
            {
                var data = await _dataContext.products.FirstOrDefaultAsync(e => e.ProductId == product.ProductId);
                if (data is null)
                    throw new Exception($"Product with ProductId {product.ProductId} not found");

                if (product.Image is not null)
                {
                    if (!string.IsNullOrEmpty(data!.ImageLocalPath))
                    {
                        var oldFilePathDirectory = Path.Combine(Directory.GetCurrentDirectory(), data.ImageLocalPath);
                        FileInfo file = new FileInfo(oldFilePathDirectory);
                        if (file.Exists)
                        {
                            file.Delete();
                        }
                    }
                    string fileName = data.ProductId + Path.GetExtension(product.Image.FileName);
                    string filePath = @"wwwroot\ProductImages\" + fileName;
                    var filePathDirectory = Path.Combine(Directory.GetCurrentDirectory(), filePath);
                    using (var fileStream = new FileStream(filePathDirectory, FileMode.Create))
                    {
                        product.Image.CopyTo(fileStream);
                    }
                    var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.Value}{HttpContext.Request.PathBase.Value}";
                    data.ImageLocalPath = filePath;
                    data.ImageUrl = baseUrl + "/ProductImages/" + fileName;
                }

                //var isCaching = _dataContext.coupons.Local.FirstOrDefault(e => e.CouponId == coupon.CouponId);
                //if(isCaching is not null)
                //	_dataContext.Entry(data).State = EntityState.Detached;
                //_dataContext.coupons.Update(_autoMapper.Map<Coupon>(coupon));

                data.Name = product.Name;
                data.Price = product.Price;
                data.Description = product.Description;
                data.CategoryName = product.CategoryName;
                await _dataContext.SaveChangesAsync();
                responce.Data = _autoMapper.Map<ProductsDTO>(data);

            }
            catch (Exception Ex)
            {
                responce.Message = Ex.Message;
                responce.IsSuccess = false;
            }
            return Ok(responce);
        }

        [HttpDelete("DeleteProduct/{Id}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<ActionResult<ServiceResponce<List<ProductsDTO>>>> DeleteProduct(Guid Id)
        {
            var responce = new ServiceResponce<List<ProductsDTO>>();
            try
            {
                var data = await _dataContext.products.FirstOrDefaultAsync(e => e.ProductId == Id);
                if(!string.IsNullOrEmpty(data!.ImageLocalPath))
                {
                    var oldFilePathDirectory = Path.Combine(Directory.GetCurrentDirectory(), data.ImageLocalPath);
                    FileInfo file = new FileInfo(oldFilePathDirectory);
                    if(file.Exists)
                    {
                        file.Delete();
                    }
                }
                if (data is null)
                    throw new Exception($"Product with ProductId {Id} not found");
                _dataContext.products.Remove(data);
                await _dataContext.SaveChangesAsync();
                responce.Data = _autoMapper.Map<List<ProductsDTO>>(_dataContext.products);
            }
            catch (Exception Ex)
            {
                responce.IsSuccess = false;
                responce.Message = Ex.Message;
            }
            return Ok(responce);
        }
    }
}
