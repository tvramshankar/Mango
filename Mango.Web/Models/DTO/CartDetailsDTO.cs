using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mango.Web.Models.DTO
{
	public class CartDetailsDTO
	{
        public int CartDetailsId { get; set; }
        public int CartHeaderId { get; set; }
        public CartHeaderDTO? CartHeader { get; set; }
        public Guid ProductId { get; set; }
        public ProductsDTO? Product { get; set; }
        public int Count { get; set; }
    }
}

