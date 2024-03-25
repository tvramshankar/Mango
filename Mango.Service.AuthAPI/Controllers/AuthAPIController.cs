using Mango.MessageBus;
using Mango.Service.AuthAPI.Models.DTO;
using Mango.Service.AuthAPI.Models.ResponseModel;
using Mango.Service.AuthAPI.Service;
using Microsoft.AspNetCore.Mvc;

namespace Mango.Service.AuthAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthAPIController : ControllerBase
    {
        private readonly IAuthService _authService;
        private ServiceResponce<object> _responceDTO;
        private readonly IMessageBus _messageBus;
        private readonly IConfiguration _configuration;
        public AuthAPIController(IAuthService authService, IMessageBus messageBus, IConfiguration configuration)
        {
            _authService = authService;
            _responceDTO = new();
            _messageBus = messageBus;
            _configuration = configuration;
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

            await _messageBus.PublishMessage(registrationRequestDTO.Email, _configuration.GetValue<string>("TopicAndQueueNames:RegisterUserQueue")!);
            return Ok(_responceDTO);
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginRequestDTO loginRequestDTO)
        {
            var loginResponce = await _authService.Login(loginRequestDTO);
            if(loginResponce.User is null)
            {
                _responceDTO.IsSuccess = false;
                _responceDTO.Message = "UserName or Password is incorrect";
                return BadRequest(_responceDTO);
            }
            _responceDTO.Data = loginResponce;
            return Ok(_responceDTO);
        }

        [HttpPost("AssignRole")]
        public async Task<IActionResult> AssignRole(RegistrationRequestDTO registrationRequestDTO)
        {
            var isAssignRoleSuccessfull = await _authService.AssignRole(registrationRequestDTO.Email, registrationRequestDTO.RoleName.ToUpper());
            if (!isAssignRoleSuccessfull)
            {
                _responceDTO.IsSuccess = false;
                _responceDTO.Message = "Error Encountered";
                return BadRequest(_responceDTO);
            }
            return Ok(_responceDTO);
        }

    }
}
