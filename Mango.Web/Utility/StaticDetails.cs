﻿using System;
namespace Mango.Web.Utility
{
	public class StaticDetails
	{
		public static string CouponAPIBase { get; set; } = string.Empty;
		public static string AuthAPIBase { get; set; } = string.Empty;
        public static string ProductAPIBase { get; set; } = string.Empty;
        public static string ShoppingCartAPIBase { get; set; } = string.Empty;
        public static string OrderAPIBase { get; set; } = string.Empty;
        public static string RoleAdmin { get; set; } = "ADMIN";
		public static string RoleCustomer { get; set; } = "CUSTOMER";
		public static string TokenCookie { get; set; } = "JWTToken";

        public const string Status_Pending = "Pending";
        public const string Status_Approved = "Approved";
        public const string Status_ReadyForPickup = "ReadyForPickup";
        public const string Status_Completed = "Completed";
        public const string Status_Refunded = "Refunded";
        public const string Status_Cancelled = "Cancelled";

		public enum ContentType
		{
			Json,
			MultipartFormData
		}

        public enum ApiType
		{
			GET,
			POST,
			PUT,
			DELETE
		}
	}
}

