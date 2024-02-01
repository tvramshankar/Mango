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
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        public AuthService(DataContext dataContext, UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager, IJwtTokenGenerator jwtTokenGenerator)

		{
            _dataContext = dataContext;
            _userManager = userManager;
            _roleManager = roleManager;
            _jwtTokenGenerator = jwtTokenGenerator;
		}

        public async Task<bool> AssignRole(string email, string roleName)
        {
            var user = _dataContext.ApplicationUsers
                .FirstOrDefault(u => u.Email!.ToLower() == email.ToLower());
            if(user is not null)
            {
                if(!_roleManager.RoleExistsAsync(roleName).GetAwaiter().GetResult()) //we can use this insted of await
                {
                    await _roleManager.CreateAsync(new IdentityRole(roleName));
                }
                await _userManager.AddToRoleAsync(user, roleName);
                return true;
            }
            return false;
        }

        public async Task<LoginResponceDTO> Login(LoginRequestDTO loginRequestDTO)
        {
            var user = _dataContext.ApplicationUsers
                .FirstOrDefault(u => u.UserName!.ToLower() == loginRequestDTO.UserName.ToLower());
            bool isValid = await _userManager.CheckPasswordAsync(user, loginRequestDTO.Password);
            if(user is null || !isValid)
            {
                return new LoginResponceDTO();
            }
            //if logged in generate token
            var roles = await _userManager.GetRolesAsync(user); //a user ca have multiple roles
            var token = _jwtTokenGenerator.GenerateToken(user, roles);
            UserDTO userDTO = new()
            {
                Email = user.Email!,
                Id = user.Id,
                Name = user.Name,
                PhoneNumber = user.PhoneNumber!
            };

            LoginResponceDTO loginResponce = new()
            {
                User = userDTO,
                Token = token
            };
            return loginResponce;
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

