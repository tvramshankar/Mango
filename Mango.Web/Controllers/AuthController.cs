using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mango.Web.Models;
using Mango.Web.Models.DTO;
using Mango.Web.Service.IService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Mango.Web.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            LoginRequestDTO loginRequestDTO = new(); 
            return View(loginRequestDTO);
        }

        [HttpPost]
        public IActionResult Login(LoginRequestDTO loginRequestDTO)
        {
            //LoginRequestDTO loginRequestDTO = new();
            return View(loginRequestDTO);
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

        public IActionResult Logout()
        {
            return View();
        }
    }
}