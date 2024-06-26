﻿using System;
namespace Mango.Service.AuthAPI.Models.DTO
{
	public class RegistrationRequestDTO
	{
        public string Email { get; set; } = string.Empty;
        public string RoleName { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}

