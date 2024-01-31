using System;
namespace Mango.Web.Models.DTO
{
	public class LoginResponceDTO
	{
		public UserDTO? User { get; set; }
		public string Token { get; set; } = string.Empty;
	}
}

