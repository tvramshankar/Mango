using System;
using Mango.Service.AuthAPI.Models;

namespace Mango.Service.AuthAPI.Service
{
	public interface IJwtTokenGenerator
	{
		string GenerateToken(ApplicationUser applicationUser);
	}
}

