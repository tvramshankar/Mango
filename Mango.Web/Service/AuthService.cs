using System;
using Mango.Web.Models;
using Mango.Web.Models.DTO;
using Mango.Web.Service.IService;

namespace Mango.Web.Service
{
	public class AuthService : IAuthService
	{
        private readonly IBaseService _baseService;
		public AuthService(IBaseService baseService)
		{
            _baseService = baseService;
		}

        public async Task<ServiceResponce<object>> AssignRoleAsync(RegistrationRequestDTO registrationRequestDTO)
        {
            return await _baseService.SendAsync(new ServiceRequest()
            {
                ApiType = Utility.StaticDetails.ApiType.POST,
                Data = registrationRequestDTO,
                Url = Utility.StaticDetails.AuthAPIBase + "/api/AuthAPI/AssignRole",
            });
        }

        public async Task<ServiceResponce<object>> LoginAsync(LoginRequestDTO loginRequestDTO)
        {
            return await _baseService.SendAsync(new ServiceRequest()
            {
                ApiType = Utility.StaticDetails.ApiType.POST,
                Data = loginRequestDTO,
                Url = Utility.StaticDetails.AuthAPIBase + "/api/AuthAPI/Login",
            });
        }

        public async Task<ServiceResponce<object>> RegisterAsync(RegistrationRequestDTO registrationRequestDTO)
        {
            return await _baseService.SendAsync(new ServiceRequest()
            {
                ApiType = Utility.StaticDetails.ApiType.POST,
                Data = registrationRequestDTO,
                Url = Utility.StaticDetails.AuthAPIBase + "/api/AuthAPI/Register",
            });
        }
    }
}

