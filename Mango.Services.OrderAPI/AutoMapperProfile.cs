using AutoMapper;
using Mango.Services.OrderAPI.Model;
using Mango.Services.OrderAPI.Model.DTO;
using Mango.Services.OrderAPI.Models.DTO;

namespace Mango.Services.ShoppingCartAPI
{
	public class AutoMapperProfile : Profile
	{
		public AutoMapperProfile()
		{
			CreateMap<OrderHeaderDTO, CartHeaderDTO>()
				.ForMember(dest => dest.CartTotal, u => u.MapFrom(src => src.OrderTotal)).ReverseMap();
            //reverse map can help in mapping in reverse order. No need to create a new
			//map in reverse order CreateMap<CartHeaderDTO, OrderHeaderDTO>().

			CreateMap<CartDetailsDTO, OrderDetailsDTO>()
				.ForMember(dest => dest.ProductName, u => u.MapFrom(src => src.Product!.Name))
				.ForMember(dest => dest.Price, u => u.MapFrom(src => src.Product!.Price)).ReverseMap();

            //here mapping product name/Price from the source ie, CartDetailsDTO.Product.Name/CartDetailsDTO.Product.Price
            //to OrderDetailsDTO.ProductName && OrderDetailsDTO.Price

            CreateMap<OrderDetailsDTO, CartDetailsDTO>();

			CreateMap<OrderHeader, OrderHeaderDTO>();

			CreateMap<OrderHeaderDTO, OrderHeader>();

            CreateMap<OrderDetails, OrderDetailsDTO>();

            CreateMap<OrderDetailsDTO, OrderDetails>();
        }

    }
}

