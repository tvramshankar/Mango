using System;
namespace MangoAPI.Models.DTO
{
	public class CouponPostDTO
	{
        public string CouponCode { get; set; } = string.Empty;
        public double DiscountAmount { get; set; }
        public int MinAmount { get; set; }
    }
}

