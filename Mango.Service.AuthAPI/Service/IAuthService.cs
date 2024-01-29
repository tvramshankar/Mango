using System;
using Mango.Service.AuthAPI.Models.DTO;

namespace Mango.Service.AuthAPI.Service
{
	public interface IAuthService
	{
		Task<UserDTO> Register(RegistrationRequestDTO registrationRequestDTO);
		Task<LoginResponceDTO> Login(LoginRequestDTO loginRequestDTO);
	}
}

