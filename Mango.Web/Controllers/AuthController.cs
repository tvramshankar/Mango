using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Mango.Web.Models;
using Mango.Web.Models.DTO;
using Mango.Web.Service.IService;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;

namespace Mango.Web.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;
        private readonly ITokenProvider _tokenProvider;
        public AuthController(IAuthService authService, ITokenProvider tokenProvider)
        {
            _authService = authService;
            _tokenProvider = tokenProvider;
        }

        [HttpGet]
        public IActionResult Login()
        {
            LoginRequestDTO loginRequestDTO = new(); 
            return View(loginRequestDTO);
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginRequestDTO loginRequestDTO)
        {
            ServiceResponce<object> serviceResponce = await _authService.LoginAsync(loginRequestDTO);
            if (serviceResponce.IsSuccess)
            {
                LoginResponceDTO loginResponceDTO = JsonConvert.DeserializeObject<LoginResponceDTO>(Convert
                    .ToString(serviceResponce.Data)!)!;
                await SignInAsync(loginResponceDTO);
                _tokenProvider.SetToken(loginResponceDTO.Token);
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError("CustomError", serviceResponce.Message);
                return View(loginRequestDTO);
            }
        }

        [HttpGet]
        public IActionResult Register()
        {
            var roleList = new List<SelectListItem>()
            {
                new SelectListItem{ Text = Utility.StaticDetails.RoleAdmin, Value = Utility.StaticDetails.RoleAdmin },
                new SelectListItem{ Text = Utility.StaticDetails.RoleCustomer, Value = Utility.StaticDetails.RoleCustomer },
            };
            ViewBag.RoleList = roleList;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegistrationRequestDTO registrationRequestDTO)
        {
            ServiceResponce<object> serviceResponce = await _authService.RegisterAsync(registrationRequestDTO);
            if(serviceResponce.IsSuccess)
            {
                if (string.IsNullOrEmpty(registrationRequestDTO.RoleName))
                    registrationRequestDTO.RoleName = Utility.StaticDetails.RoleCustomer;
                ServiceResponce<object> assignRoleResponce = await _authService.AssignRoleAsync(registrationRequestDTO);
                if(assignRoleResponce.IsSuccess)
                {
                    TempData["success"] = "Registration Successfull";
                    return RedirectToAction(nameof(Login));
                }
            }
            var roleList = new List<SelectListItem>()
            {
                new SelectListItem{ Text = Utility.StaticDetails.RoleAdmin, Value = Utility.StaticDetails.RoleAdmin },
                new SelectListItem{ Text = Utility.StaticDetails.RoleCustomer, Value = Utility.StaticDetails.RoleCustomer },
            };
            ViewBag.RoleList = roleList;
            return View(registrationRequestDTO);
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            _tokenProvider.ClearToken();
            return RedirectToAction(nameof(Login));
        }

        private async Task SignInAsync(LoginResponceDTO loginResponceDTO)
        {
            var handler = new JwtSecurityTokenHandler();

            var jwt = handler.ReadJwtToken(loginResponceDTO.Token);

            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);

            identity.AddClaim(new Claim(JwtRegisteredClaimNames.Email,
                jwt.Claims.FirstOrDefault(e=> e.Type == JwtRegisteredClaimNames.Email)!.Value));
            identity.AddClaim(new Claim(JwtRegisteredClaimNames.Sub,
                jwt.Claims.FirstOrDefault(e => e.Type == JwtRegisteredClaimNames.Sub)!.Value));
            identity.AddClaim(new Claim(JwtRegisteredClaimNames.Name,
                jwt.Claims.FirstOrDefault(e => e.Type == JwtRegisteredClaimNames.Name)!.Value));

            identity.AddClaim(new Claim(ClaimTypes.Name,
               jwt.Claims.FirstOrDefault(e => e.Type == JwtRegisteredClaimNames.Email)!.Value));



            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

        }
    }
}