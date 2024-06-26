﻿
namespace Mango.Services.ProductAPI.Models.DTO
{
	public class ProductsDTO
	{
        public Guid ProductId { get; set; }
        public string Name { get; set; } = string.Empty;
        public double Price { get; set; }
        public string Description { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public string ImageLocalPath { get; set; } = string.Empty;
        public IFormFile? Image { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
    }
}

