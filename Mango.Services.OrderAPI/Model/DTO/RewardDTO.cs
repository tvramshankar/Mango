using System;
namespace Mango.Services.OrderAPI.Model.DTO
{
	public class RewardDTO
	{
        public string UserId { get; set; }
        public int RewardActivity { get; set; }
        public int OrderId { get; set; }
    }
}