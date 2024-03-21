using System;
using Azure.Messaging.ServiceBus;
using Mango.Service.RewardAPI.Message;
using Mango.Service.RewardAPI.Service;
using Newtonsoft.Json;

namespace Mango.Service.RewardAPI.Messaging
{
	public class AzureServiceBusConsumer : IAzureServiceBusConsumer
	{
		private readonly string serviceBusConnectionString;
		private readonly string orderCreatedTopic;
		private readonly string orderCreatedRewardSubscription;
        private readonly IConfiguration _configuration;
		private readonly ServiceBusProcessor _rewardProcessor;
        private readonly RewardService rewardService;

		public AzureServiceBusConsumer(IConfiguration configuration, RewardService rewardService)
		{
			_configuration = configuration;
			serviceBusConnectionString = _configuration.GetValue<string>("ServiceBusConnectionString")!;
            orderCreatedTopic = _configuration.GetValue<string>("TopicAndQueueNames:OrderCreatedTopic")!;
            orderCreatedRewardSubscription = _configuration.GetValue<string>("TopicAndQueueNames:OrderCreated_Rewards_Subscription")!;

            var client = new ServiceBusClient(serviceBusConnectionString);
            _rewardProcessor = client.CreateProcessor(orderCreatedTopic, orderCreatedRewardSubscription);
            this.rewardService = rewardService;
        }

        public async Task Start()
        {
            _rewardProcessor.ProcessMessageAsync += OnNewOrderRewardsRequestReceived;
            _rewardProcessor.ProcessErrorAsync += ErrorHandler;
            await _rewardProcessor.StartProcessingAsync();
        }

        public async Task Stop()
        {
            await _rewardProcessor.StopProcessingAsync();
            await _rewardProcessor.DisposeAsync();
        }

        private Task ErrorHandler(ProcessErrorEventArgs args)
        {
            Console.WriteLine(args.Exception.ToString());
            return Task.CompletedTask;
        }

        private async Task OnNewOrderRewardsRequestReceived(ProcessMessageEventArgs args)
        {
            var message = args.Message;
            var body = System.Text.Encoding.UTF8.GetString(message.Body);
            RewardMessage rewardMessage = JsonConvert.DeserializeObject<RewardMessage>(body)!;
            try
            {
                await this.rewardService.UpdateReward(rewardMessage)!;
                await args.CompleteMessageAsync(args.Message);
            }
            catch
            {
                throw;
            }
        }
    }
}

