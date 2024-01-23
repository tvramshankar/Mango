﻿using System;
namespace Mango.Web.Models.DTO
{
	public class CouponDTO
	{
        public int CouponId { get; set; }
        public string CouponCode { get; set; } = string.Empty;
        public double DiscountAmount { get; set; }
        public int MinAmount { get; set; }
    }
}

