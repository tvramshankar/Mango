using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Mango.Services.ShoppingCartAPI.Data;
using Mango.Services.ShoppingCartAPI.Models;
using Mango.Services.ShoppingCartAPI.Models.DTO;
using Mango.Services.ShoppingCartAPI.Models.ResponseModel;
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

        public ShoppingCartAPIController(IMapper mapper, DataContext dataContext)
        {
            _mapper = mapper;
            _dataContext = dataContext;
            _serviceResponce = new();
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
    }
}
