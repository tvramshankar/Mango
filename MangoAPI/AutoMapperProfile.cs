using System;
using AutoMapper;
using MangoAPI.Models;
using MangoAPI.Models.DTO;
namespace MangoAPI
{
	public class AutoMapperProfile : Profile
	{
		public AutoMapperProfile()
		{
			CreateMap<Coupon, CouponDTO>();
            CreateMap<CouponDTO, Coupon>();
            CreateMap<CouponPostDTO, Coupon>();
        }
	}
}

