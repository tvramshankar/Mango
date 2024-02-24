using System;
using AutoMapper;
using Mango.Services.ProductAPI.Models;
using Mango.Services.ProductAPI.Models.DTO;
namespace Mango.Services.ProductAPI
{
	public class AutoMapperProfile : Profile
	{
		public AutoMapperProfile()
		{
			CreateMap<Product, ProductsDTO>();
            CreateMap<ProductsDTO, Product>();
            CreateMap<ProductPostDTO, Product>();
        }

	}
}

