﻿using System;
namespace Mango.Web.Utility
{
	public class StaticDetails
	{
		public static string CouponAPIBase { get; set; } = string.Empty;
		public static string AuthAPIBase { get; set; } = string.Empty;
        public static string ProductAPIBase { get; set; } = string.Empty;
        public static string ShoppingCartAPIBase { get; set; } = string.Empty;
        public static string RoleAdmin { get; set; } = "ADMIN";
		public static string RoleCustomer { get; set; } = "CUSTOMER";
		public static string TokenCookie { get; set; } = "JWTToken";
        public enum ApiType
		{
			GET,
			POST,
			PUT,
			DELETE
		}
	}
}

