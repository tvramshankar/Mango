using System;
using Mango.Web.Models;
using Mango.Web.Models.DTO;

namespace Mango.Web.Service.IService
{
	public interface IAuthService
	{
		Task<ServiceResponce<object>> LoginAsync(LoginRequestDTO loginRequestDTO);
		Task<ServiceResponce<object>> RegisterAsync(RegistrationRequestDTO registrationRequestDTO);
		Task<ServiceResponce<object>> AssignRoleAsync(RegistrationRequestDTO registrationRequestDTO);
    }
}

