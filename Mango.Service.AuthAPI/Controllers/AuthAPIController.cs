using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mango.Service.AuthAPI.Models.DTO;
using Mango.Service.AuthAPI.Models.ResponseModel;
using Mango.Service.AuthAPI.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Mango.Service.AuthAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthAPIController : ControllerBase
    {
        private readonly IAuthService _authService;
        private ServiceResponce<object> _responceDTO;
        public AuthAPIController(IAuthService authService)
        {
            _authService = authService;
            _responceDTO = new();
        }
        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegistrationRequestDTO registrationRequestDTO)
        {
            var errMessage = await _authService.Register(registrationRequestDTO);
            if(!string.IsNullOrEmpty(errMessage))
            {
                _responceDTO.IsSuccess = false;
                _responceDTO.Message = errMessage;
                return BadRequest(_responceDTO);
            }
            return Ok(_responceDTO);
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login()
        {
            return Ok();
        }

    }
}
