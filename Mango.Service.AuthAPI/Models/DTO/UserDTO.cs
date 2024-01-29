using System;
namespace Mango.Service.AuthAPI.Models.DTO
{
	public class UserDTO
	{
		public int Id { get; set; }
		public string Email { get; set; } = string.Empty;
		public string Name { get; set; } = string.Empty;
		public string PhoneNumber { get; set; } = string.Empty;
	}
}

