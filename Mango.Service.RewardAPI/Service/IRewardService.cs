using System;
using Mango.Service.RewardAPI.Message;

namespace Mango.Service.RewardAPI.Service
{
	public interface IRewardService
	{
		Task UpdateReward(RewardMessage rewardMessage);
    }
}
