﻿using System;
namespace Mango.Web.Models.DTO
{
	public class UserDTO
	{
		public string Id { get; set; }
		public string Email { get; set; } = string.Empty;
		public string Name { get; set; } = string.Empty;
		public string PhoneNumber { get; set; } = string.Empty;
	}
}

