using System;
using Microsoft.AspNetCore.Mvc;
using MangoAPI.Data;
using MangoAPI.Models.ResponseModel;
using MangoAPI.Models.DTO;
using Microsoft.EntityFrameworkCore;
using MangoAPI.Models;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;

namespace MangoAPI.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	[Authorize]
	public class CouponAPIController : ControllerBase
	{
		private readonly DataContext _dataContext;
		private readonly IMapper _autoMapper;
		public CouponAPIController(DataContext dataContext, IMapper autoMapper)
		{
			_dataContext = dataContext;
			_autoMapper = autoMapper;
		}

		[HttpGet("GetAllCoupon")]
		public async Task<ActionResult<ServiceResponce<List<CouponDTO>>>> Get()
		{
			var responce = new ServiceResponce<List<CouponDTO>>();
            try
            {
                var data = await _dataContext.coupons.ToListAsync();
                responce.Data = _autoMapper.Map<List<CouponDTO>>(data);
            }
            catch (Exception Ex)
            {
                responce.Message = Ex.Message;
                responce.IsSuccess = false;
            }
			return Ok(responce);
		}
		[HttpGet("GetById/{Id}")]
		public async Task<ActionResult<ServiceResponce<CouponDTO>>> GetCouponById(int Id)
		{
			var responce = new ServiceResponce<CouponDTO>();
            try
            {
                var data = await _dataContext
                .coupons
                .FirstOrDefaultAsync(e => e.CouponId == Id);
                responce.Data = _autoMapper.Map<CouponDTO>(data);
            }
            catch (Exception Ex)
            {
                responce.Message = Ex.Message;
                responce.IsSuccess = false;
            }
			return Ok(responce);
		}
		[HttpGet("GetByCode/{Code}")]
		public async Task<ActionResult<ServiceResponce<CouponDTO>>> GetCouponById(string Code)
		{
			var responce = new ServiceResponce<CouponDTO>();
            try
            {
                var data = await _dataContext
                .coupons
                .FirstOrDefaultAsync(e => e.CouponCode.ToLower() == Code.ToLower());
                responce.Data = _autoMapper.Map<CouponDTO>(data);
            }
            catch (Exception Ex)
            {
                responce.Message = Ex.Message;
                responce.IsSuccess = false;
            }
			return Ok(responce);
		}
		[HttpPost("AddCoupon")]
		[Authorize(Roles ="ADMIN")]
		public async Task<ActionResult<ServiceResponce<CouponDTO>>> AddCoupon(CouponPostDTO coupon)
		{
			var responce = new ServiceResponce<CouponDTO>();
			try
			{
                var data = _autoMapper.Map<Coupon>(coupon);
                _dataContext.coupons.Add(data);
                await _dataContext.SaveChangesAsync();
                responce.Data = _autoMapper.Map<CouponDTO>(await _dataContext
                    .coupons
                    .FirstOrDefaultAsync(e => e.CouponId == data.CouponId));
            }
			catch(Exception Ex)
			{
                responce.Message = Ex.Message;
                responce.IsSuccess = false;
            }
			return Ok(responce);
		}
		[HttpPut("UpdateCoupon")]
        [Authorize(Roles = "ADMIN")]
        public async Task<ActionResult<ServiceResponce<CouponDTO>>> UpdateCoupon(CouponDTO coupon)
		{
            var responce = new ServiceResponce<CouponDTO>();
			try
			{
				var data = await _dataContext.coupons.FirstOrDefaultAsync(e => e.CouponId == coupon.CouponId);
				if (data is null)
					throw new Exception($"Coupon with CouponId {coupon.CouponId} not found");
				//var isCaching = _dataContext.coupons.Local.FirstOrDefault(e => e.CouponId == coupon.CouponId);
				//if(isCaching is not null)
				//	_dataContext.Entry(data).State = EntityState.Detached;
				//_dataContext.coupons.Update(_autoMapper.Map<Coupon>(coupon));
				data.CouponCode = coupon.CouponCode;
				data.DiscountAmount = coupon.DiscountAmount;
				data.MinAmount = coupon.MinAmount;
				await _dataContext.SaveChangesAsync();
				responce.Data =_autoMapper.Map<CouponDTO>(data);

            }
			catch (Exception Ex)
			{
				responce.Message = Ex.Message;
				responce.IsSuccess = false;
			}
			return Ok(responce);
        }
        [HttpDelete("DeleteCoupon/{Id}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<ActionResult<ServiceResponce<List<CouponDTO>>>> DeleteCoupon(int Id)
		{
			var responce = new ServiceResponce<List<CouponDTO>>();
			try
			{
				var data = await _dataContext.coupons.FirstOrDefaultAsync(e => e.CouponId == Id);
				if (data is null)
					throw new Exception($"Coupon with CouponId {Id} not found");
				_dataContext.coupons.Remove(data);
				await _dataContext.SaveChangesAsync();
				responce.Data = _autoMapper.Map<List<CouponDTO>>(_dataContext.coupons);
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

