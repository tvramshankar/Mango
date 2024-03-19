using System;
using System.ComponentModel.DataAnnotations.Schema;
using Mango.Services.OrderAPI.Models.DTO;

namespace Mango.Services.OrderAPI.Model.DTO
{
	public class OrderDetailsDTO
	{
        public int OrderDetailsId { get; set; }
        public int OrderHeaderId { get; set; }
        public Guid ProductId { get; set; }
        public ProductsDTO? Product { get; set; }
        public int Count { get; set; }
        public string ProductName { get; set; }
        public double Price { get; set; }
    }
}

