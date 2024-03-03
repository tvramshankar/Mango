﻿
using System.ComponentModel.DataAnnotations;

namespace Mango.Web.Models.DTO
{
	public class ProductsDTO
	{
        public Guid ProductId { get; set; }
        public string Name { get; set; } = string.Empty;
        public double Price { get; set; }
        public string Description { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        [Range(1,100)]
        public int ProductCount { get; set; } = 1;
    }
}

