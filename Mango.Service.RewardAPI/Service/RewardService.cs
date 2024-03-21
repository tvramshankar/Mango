using System;
using System.Text;
using Mango.Service.RewardAPI.Data;
using Mango.Service.RewardAPI.Message;
using Mango.Service.RewardAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace Mango.Service.RewardAPI.Service
{
	public class RewardService : IRewardService
    {
        private DbContextOptions<DataContext> _dbOptions;

        public RewardService(DbContextOptions<DataContext> dbOptions)
        {
            _dbOptions = dbOptions;
        }

        //here DbContextOptions is used because
        //we are using RewardService as services.addsigleton in program.cs
        //so we cant use scopped dbcontext(dbcontext is scopped by default) in 
        //singleton RewardService(we registed RewardService as singleton) so we
        //are implementing like this using DbContextOptions

        public async Task UpdateReward(RewardMessage rewardMessage)
        {
            try
            {
                Reward reward = new()
                {
                    OrderId = rewardMessage.OrderId,
                    RewardActivity = rewardMessage.RewardActivity,
                    UserId = rewardMessage.UserId,
                    RewardsDate = DateTime.Now
                };
                await using var _db = new DataContext(_dbOptions);
                _db.Rewards.Add(reward);
                await _db.SaveChangesAsync();
            }
            catch
            {
                throw;
            }
        }
    }
}

