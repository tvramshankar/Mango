using System;
using Mango.Service.AuthAPI.Data;
using Mango.Service.AuthAPI.Models;
using Mango.Service.AuthAPI.Models.DTO;
using Microsoft.AspNetCore.Identity;

namespace Mango.Service.AuthAPI.Service
{
	public class AuthService : IAuthService
	{
        private readonly DataContext _dataContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
		public AuthService(DataContext dataContext, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)

		{
            _dataContext = dataContext;
            _userManager = userManager;
            _roleManager = roleManager;
		}

        public Task<LoginResponceDTO> Login(LoginRequestDTO loginRequestDTO)
        {
            throw new NotImplementedException();
        }

        public async Task<string> Register(RegistrationRequestDTO registrationRequestDTO)
        {
            ApplicationUser applicationUser = new()
            {
                UserName = registrationRequestDTO.Email,
                Email = registrationRequestDTO.Email,
                NormalizedEmail = registrationRequestDTO.Email.ToUpper(),
                PhoneNumber = registrationRequestDTO.PhoneNumber,
                Name = registrationRequestDTO.Name
            };

            try
            {
                var result = await _userManager.CreateAsync(applicationUser, registrationRequestDTO.Password);
                if(result.Succeeded)
                {
                    var UserToReturn = _dataContext.ApplicationUsers.First(u => u.UserName == registrationRequestDTO.Email);
                    UserDTO userDTO = new()
                    {
                        Email = UserToReturn.Email!,
                        Id = UserToReturn.Id,
                        Name = UserToReturn.Name,
                        PhoneNumber = UserToReturn.PhoneNumber!,

                    };

                    return "";
                }
                else
                {
                    return result.Errors.FirstOrDefault()!.Description;
                }
            }
            catch(Exception ex)
            {

            }
            return "Error Encountered";
        }
    }
}

