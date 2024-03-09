using AutoMapper;
using Mango.Services.ShoppingCartAPI.Models;
using Mango.Services.ShoppingCartAPI.Models.DTO;

namespace Mango.Services.ShoppingCartAPI
{
	public class AutoMapperProfile : Profile
	{
		public AutoMapperProfile()
		{
			CreateMap<CartHeader, CartHeaderDTO>();
			CreateMap<CartHeaderDTO, CartHeader>();
            CreateMap<CartDetails, CartDetailsDTO>();
            CreateMap<CartDetailsDTO, CartDetails>();
        }

	}
}

