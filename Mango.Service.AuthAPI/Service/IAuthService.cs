using System;
using Mango.Service.AuthAPI.Models.DTO;

namespace Mango.Service.AuthAPI.Service
{
	public interface IAuthService
	{
		Task<string> Register(RegistrationRequestDTO registrationRequestDTO);
		Task<LoginResponceDTO> Login(LoginRequestDTO loginRequestDTO);
		Task<bool> AssignRole(string email, string roleName);
	}
}

