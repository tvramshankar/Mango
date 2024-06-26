﻿using System;
using Mango.Services.EmailAPI.Message;
using Mango.Services.EmailAPI.Models.DTO;

namespace Mango.Services.EmailAPI.Service
{
	public interface IEmailService
	{
		Task EmailCartAndLog(CartDTO cartDto);
		Task RegisterEmailAndLog(string email);
		Task LogOrderPlaced(RewardMessage rewardMessage);
    }
}

